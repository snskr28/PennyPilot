using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PennyPilot.Backend.Application.DTOs;
using PennyPilot.Backend.Application.Interfaces;
using PennyPilot.Backend.Application.Utilities;
using PennyPilot.Backend.Domain.Entities;
using PennyPilot.Backend.Domain.Interfaces;

namespace PennyPilot.Backend.Application.Services
{
    public class IncomeService : IIncomeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public IncomeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public async Task<ServerResponse<List<Guid>>> AddIncomesAsync(Guid userId, List<AddIncomeDto> requestDtos)
        {
            //Cache to avoid repeated DB calls within this request
            var categoryLookup = new Dictionary<string, Category>(StringComparer.OrdinalIgnoreCase);
            var mappedCategories = new HashSet<Guid>();

            var incomesToAdd = new List<Income>();
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
                            Type = "Income",
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

                // Prepare income entity
                var income = new Income
                {
                    IncomeId = Guid.NewGuid(),
                    UserId = userId,
                    CategoryId = category.CategoryId,
                    Title = dto.Title,
                    Description = dto.Description,
                    Amount = dto.Amount,
                    Date = dto.Date,
                    Source = dto.Source,
                    CreatedAt = DateTime.UtcNow,
                    IsEnabled = true,
                    IsDeleted = false
                };

                incomesToAdd.Add(income);
            }

            // Batch insert all
            if (newCategoriesToAdd.Any())
                await _unitOfWork.Categories.AddRangeAsync(newCategoriesToAdd);

            if (newUserCategoriesToAdd.Any())
                await _unitOfWork.UserCategories.AddRangeAsync(newUserCategoriesToAdd);

            if (incomesToAdd.Any())
                await _unitOfWork.Incomes.AddRangeAsync(incomesToAdd);

            await _unitOfWork.SaveChangesAsync();

            response.Data = incomesToAdd.Select(e => e.IncomeId).ToList();
            return response;
        }

        public async Task UpdateIncomeAsync(Guid userId, UpdateIncomeDto dto)
        {
            var income = await _unitOfWork.Incomes.GetByIdAsync(dto.IncomeId)
                ?? throw new Exception("Income not found");

            if(income.IsDeleted == true)
            {
                throw new Exception("Income not found");
            }

            if (income.UserId != userId)
                throw new UnauthorizedAccessException();

            string formattedCategory = dto.Category.ToPascalCase();
            var category = await _unitOfWork.Categories.GetByNameAsync(formattedCategory);
            if (category == null)
            {
                category = new Category
                {
                    CategoryId = Guid.NewGuid(),
                    Name = formattedCategory,
                    Type = "Income",
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

            income.CategoryId = category.CategoryId;
            income.Source = dto.Source;
            income.Description = dto.Description;
            income.Amount = dto.Amount;
            income.Date = dto.Date;
            income.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteIncomeAsync(Guid userId, Guid incomeId)
        {
            var income = await _unitOfWork.Incomes.GetByIdAsync(incomeId)
                ?? throw new Exception("Income not found");

            if (income.UserId != userId)
                throw new UnauthorizedAccessException();

            income.IsDeleted = true;
            income.IsEnabled = false;
            income.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<TableResponseDto<IncomeTableDto>> GetUserIncomesAsync(Guid userId, TableRequestDto requestDto)
        {
            var incomes = _unitOfWork.Incomes.AsQueryable()
                           .Where(e => e.UserId == userId && !e.IsDeleted && e.IsEnabled)
                           .Select(e => new IncomeTableDto
                           {
                              Amount = e.Amount,
                              Date = e.Date,
                              Category = e.Category.Name,
                              Description = e.Description,
                              Source = e.Source,
                              Title = e.Title
                           });

            bool descending = requestDto.SortOrder?.ToLower() == "desc";

            incomes = requestDto.SortBy?.ToLower() switch
            {
                "amount" => descending ? incomes.OrderByDescending(x => x.Amount) : incomes.OrderBy(x => x.Amount),
                "category" => descending ? incomes.OrderByDescending(x => x.Category) : incomes.OrderBy(x => x.Category),
                "description" => descending ? incomes.OrderByDescending(x => x.Description) : incomes.OrderBy(x => x.Description),
                "source" => descending ? incomes.OrderByDescending(x => x.Source) : incomes.OrderBy(x => x.Source),
                "date" => descending ? incomes.OrderByDescending(x => x.Date) : incomes.OrderBy(x => x.Date),
                "title" => descending ? incomes.OrderByDescending(x=>x.Title) : incomes.OrderBy(x=>x.Title),
                _ => incomes.OrderBy(x => x.Date) // default sorting
            };

            var totalCount = incomes.Count();
            var totalPages = (int)Math.Ceiling((double)totalCount / requestDto.PageSize);

            var paginatedData = incomes
                                .Skip((requestDto.PageNumber - 1) * requestDto.PageSize)
                                .Take(requestDto.PageSize);

            var resultData = await Task.FromResult(paginatedData.ToList());

            return new TableResponseDto<IncomeTableDto>
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
