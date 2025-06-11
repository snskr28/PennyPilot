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

        public async Task<Guid> AddIncomeAsync(Guid userId, AddIncomeDto dto)
        {
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

            // Map User and Category
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

            var income = new Income
            {
                IncomeId = Guid.NewGuid(),
                UserId = userId,
                CategoryId = category.CategoryId,
                Source = dto.Source,
                Description = dto.Description,
                Amount = dto.Amount,
                Date = dto.Date,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false,
                IsEnabled = true
            };

            await _unitOfWork.Incomes.AddAsync(income);
            await _unitOfWork.SaveChangesAsync();

            return income.IncomeId;
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
