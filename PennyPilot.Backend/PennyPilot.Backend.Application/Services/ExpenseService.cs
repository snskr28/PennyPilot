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
            var newUserCategoriesToAdd = new List<Usercategory>();

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
                            Categoryid = Guid.NewGuid(),
                            Name = formattedCategory,
                            Type = "Expense",
                            Isenabled = true,
                            Isdeleted = false
                        };
                        newCategoriesToAdd.Add(category);
                    }

                    categoryLookup[formattedCategory] = category;
                }

                // Map user to category if not already
                if (!mappedCategories.Contains(category.Categoryid))
                {
                    bool isMapped = await _unitOfWork.UserCategories.ExistsAsync(userId, category.Categoryid);
                    if (!isMapped)
                    {
                        newUserCategoriesToAdd.Add(new Usercategory
                        {
                            Usercategoryid = Guid.NewGuid(),
                            Userid = userId,
                            Categoryid = category.Categoryid
                        });
                    }
                    mappedCategories.Add(category.Categoryid);
                }

                // Prepare expense entity
                var expense = new Expense
                {
                    Expenseid = Guid.NewGuid(),
                    Userid = userId,
                    Categoryid = category.Categoryid,
                    Title = dto.Title,
                    Description = dto.Description,
                    Amount = dto.Amount,
                    Paymentmode = dto.PaymentMode,
                    Paidby = dto.PaidBy,
                    Date = dto.Date,
                    Receiptimage = dto.ReceiptImage,
                    Createdat = DateTime.UtcNow,
                    Isenabled = true,
                    Isdeleted = false
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

            response.Data = expensesToAdd.Select(e => e.Expenseid).ToList();
            return response;
        }

        public async Task UpdateExpenseAsync(Guid userId, UpdateExpenseDto requestDto)
        {
            var expense = await _unitOfWork.Expenses.GetByIdAsync(requestDto.ExpenseId)
                ?? throw new Exception("Expense not found");

            if(expense.Isdeleted == true)
            {
                throw new Exception("Expense not found");
            }

            if (expense.Userid != userId)
                throw new UnauthorizedAccessException();

            string formattedCategory = requestDto.Category.ToPascalCase();
            var category = await _unitOfWork.Categories.GetByNameAsync(formattedCategory);
            if (category == null)
            {
                category = new Category
                {
                    Categoryid = Guid.NewGuid(),
                    Name = formattedCategory,
                    Type = "Expense",
                    Isenabled = true,
                    Isdeleted = false
                };
                await _unitOfWork.Categories.AddAsync(category);
            }

            bool isMapped = await _unitOfWork.UserCategories.ExistsAsync(userId, category.Categoryid);
            if (!isMapped)
            {
                var userCategory = new Usercategory
                {
                    Usercategoryid = Guid.NewGuid(),
                    Userid = userId,
                    Categoryid = category.Categoryid
                };
                await _unitOfWork.UserCategories.AddAsync(userCategory);
            }

            expense.Title = requestDto.Title;
            expense.Description = requestDto.Description;
            expense.Amount = requestDto.Amount;
            expense.Paymentmode = requestDto.PaymentMode;
            expense.Paidby = requestDto.PaidBy;
            expense.Date = requestDto.Date;
            expense.Receiptimage = requestDto.ReceiptImage;
            expense.Categoryid = category.Categoryid;
            expense.Updatedat = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteExpenseAsync(Guid userId, Guid expenseId)
        {
            var expense = await _unitOfWork.Expenses.GetByIdAsync(expenseId)
                ?? throw new Exception("Expense not found");

            if (expense.Userid != userId)
                throw new UnauthorizedAccessException();

            expense.Isdeleted = true;
            expense.Isenabled = false;
            expense.Updatedat = DateTime.UtcNow;

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
                                PaymentMode = e.Paymentmode,
                                PaidBy = e.Paidby ?? "N/A",
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
