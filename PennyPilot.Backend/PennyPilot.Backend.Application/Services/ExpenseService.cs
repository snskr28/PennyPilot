using PennyPilot.Backend.Application.DTOs;
using PennyPilot.Backend.Application.Interfaces;
using PennyPilot.Backend.Application.Utilities;
using PennyPilot.Backend.Domain.Entities;
using PennyPilot.Backend.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PennyPilot.Backend.Application.Services
{
    public class ExpenseService : IExpenseService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFilterService _filterService;

        public ExpenseService(IUnitOfWork unitOfWork, IFilterService filterService)
        {
            _unitOfWork = unitOfWork;
            _filterService = filterService;
        }

        public async Task<ServerResponse<List<Guid>>> AddExpensesAsync(Guid userId, List<AddExpenseDto> requestDtos)
        {
            // Cache to avoid repeated DB calls within this request
            var categoryLookup = new Dictionary<string, Category>(StringComparer.OrdinalIgnoreCase);
            var mappedCategories = new HashSet<Guid>();

            var expensesToAdd = new List<Expense>();
            var newCategoriesToAdd = new List<Category>();
            var newUserCategoriesToAdd = new List<UserCategory>();

            var response = new ServerResponse<List<Guid>>
            {
                Data = new List<Guid>()
            };

            foreach (var dto in requestDtos)
            {
                string formattedCategory = dto.Category.ToPascalCase();

                // Get or add category
                if (!categoryLookup.TryGetValue(formattedCategory, out var category))
                {
                    category = await _unitOfWork.Categories.GetByNameAsync(formattedCategory);

                    if (category == null)
                    {
                        category = new Category
                        {
                            CategoryId = Guid.NewGuid(),
                            Name = formattedCategory,
                            Type = "Expense",
                            IsEnabled = true,
                            IsDeleted = false
                        };
                        newCategoriesToAdd.Add(category);
                    }

                    categoryLookup[formattedCategory] = category;
                }

                // Map user to category if not already
                if (!mappedCategories.Contains(category.CategoryId))
                {
                    bool isMapped = await _unitOfWork.UserCategories.ExistsAsync(userId, category.CategoryId);
                    if (!isMapped)
                    {
                        newUserCategoriesToAdd.Add(new UserCategory
                        {
                            UserCategoryId = Guid.NewGuid(),
                            UserId = userId,
                            CategoryId = category.CategoryId
                        });
                    }
                    mappedCategories.Add(category.CategoryId);
                }

                // Prepare expense entity
                var expense = new Expense
                {
                    ExpenseId = Guid.NewGuid(),
                    UserId = userId,
                    CategoryId = category.CategoryId,
                    Title = dto.Title,
                    Description = dto.Description,
                    Amount = dto.Amount,
                    PaymentMode = dto.PaymentMode,
                    PaidBy = dto.PaidBy,
                    Date = dto.Date,
                    ReceiptImage = dto.ReceiptImage,
                    CreatedAt = DateTime.UtcNow,
                    IsEnabled = true,
                    IsDeleted = false
                };

                expensesToAdd.Add(expense);
            }

            // Batch insert all
            if (newCategoriesToAdd.Any())
                await _unitOfWork.Categories.AddRangeAsync(newCategoriesToAdd);

            if (newUserCategoriesToAdd.Any())
                await _unitOfWork.UserCategories.AddRangeAsync(newUserCategoriesToAdd);

            if (expensesToAdd.Any())
                await _unitOfWork.Expenses.AddRangeAsync(expensesToAdd);

            await _unitOfWork.SaveChangesAsync();

            response.Data = expensesToAdd.Select(e => e.ExpenseId).ToList();
            return response;
        }

        public async Task UpdateExpenseAsync(Guid userId, UpdateExpenseDto requestDto)
        {
            var expense = await _unitOfWork.Expenses.GetByIdAsync(requestDto.ExpenseId)
                ?? throw new Exception("Expense not found");

            if(expense.IsDeleted == true)
            {
                throw new Exception("Expense not found");
            }

            if (expense.UserId != userId)
                throw new UnauthorizedAccessException();

            string formattedCategory = requestDto.Category.ToPascalCase();
            var category = await _unitOfWork.Categories.GetByNameAsync(formattedCategory);
            if (category == null)
            {
                category = new Category
                {
                    CategoryId = Guid.NewGuid(),
                    Name = formattedCategory,
                    Type = "Expense",
                    IsEnabled = true,
                    IsDeleted = false
                };
                await _unitOfWork.Categories.AddAsync(category);
            }

            bool isMapped = await _unitOfWork.UserCategories.ExistsAsync(userId, category.CategoryId);
            if (!isMapped)
            {
                var userCategory = new UserCategory
                {
                    UserCategoryId = Guid.NewGuid(),
                    UserId = userId,
                    CategoryId = category.CategoryId
                };
                await _unitOfWork.UserCategories.AddAsync(userCategory);
            }

            expense.Title = requestDto.Title;
            expense.Description = requestDto.Description;
            expense.Amount = requestDto.Amount;
            expense.PaymentMode = requestDto.PaymentMode;
            expense.PaidBy = requestDto.PaidBy;
            expense.Date = requestDto.Date;
            expense.ReceiptImage = requestDto.ReceiptImage;
            expense.CategoryId = category.CategoryId;
            expense.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteExpenseAsync(Guid userId, Guid expenseId)
        {
            var expense = await _unitOfWork.Expenses.GetByIdAsync(expenseId)
                ?? throw new Exception("Expense not found");

            if (expense.UserId != userId)
                throw new UnauthorizedAccessException();

            expense.IsDeleted = true;
            expense.IsEnabled = false;
            expense.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<TableResponseDto<ExpenseTableDto>> GetUserExpensesAsync(Guid userId, TableRequestDto requestDto)
        {
            var expenses = _filterService.GetFilteredExpenses(_unitOfWork.Expenses.AsQueryable(), userId, requestDto.DashboardFilter)
                            .Select(e => new ExpenseTableDto
                            {
                                Title = e.Title,
                                Description = e.Description,
                                Amount = e.Amount,
                                Category = e.Category.Name,
                                PaymentMode = e.PaymentMode,
                                PaidBy = e.PaidBy ?? "N/A",
                                Date = e.Date
                            });

            bool descending = requestDto.SortOrder?.ToLower() == "desc";

            expenses = requestDto.SortBy?.ToLower() switch
            {
                "title" => descending ? expenses.OrderByDescending(x => x.Title) : expenses.OrderBy(x => x.Title),
                "amount" => descending ? expenses.OrderByDescending(x => x.Amount) : expenses.OrderBy(x => x.Amount),
                "category" => descending ? expenses.OrderByDescending(x => x.Category) : expenses.OrderBy(x => x.Category),
                "description" => descending ? expenses.OrderByDescending(x => x.Description) : expenses.OrderBy(x => x.Description),
                "paymentmode" => descending ? expenses.OrderByDescending(x => x.PaymentMode) : expenses.OrderBy(x => x.PaymentMode),
                "paidby" => descending ? expenses.OrderByDescending(x => x.PaidBy) : expenses.OrderBy(x => x.PaidBy),
                "date" => descending ? expenses.OrderByDescending(x => x.Date) : expenses.OrderBy(x => x.Date),
                _ => expenses.OrderBy(x => x.Date) // default sorting
            };

            var totalCount = expenses.Count();
            var totalPages = (int)Math.Ceiling((double)totalCount / requestDto.PageSize);

            var paginatedData = expenses
                                .Skip((requestDto.PageNumber - 1) * requestDto.PageSize)
                                .Take(requestDto.PageSize);

            var resultData = await Task.FromResult(paginatedData.ToList());

            return new TableResponseDto<ExpenseTableDto>
            {
                Items = resultData,
                TotalCount = totalCount,
                TotalPages = totalPages,
                PageNumber = requestDto.PageNumber,
                PageSize = requestDto.PageSize
            };
        }
    }
}
