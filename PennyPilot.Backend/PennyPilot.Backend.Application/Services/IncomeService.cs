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
        private readonly IFilterService _filterService;

        public IncomeService(IUnitOfWork unitOfWork, IFilterService filterService)
        {
            _unitOfWork = unitOfWork;
            _filterService = filterService;
        }


        public async Task<ServerResponse<List<Guid>>> AddIncomesAsync(Guid userId, List<AddIncomeDto> requestDtos)
        {
            //Cache to avoid repeated DB calls within this request
            var categoryLookup = new Dictionary<string, Category>(StringComparer.OrdinalIgnoreCase);
            var mappedCategories = new HashSet<Guid>();

            var incomesToAdd = new List<Income>();
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
                            Type = "Income",
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

                // Prepare income entity
                var income = new Income
                {
                    Incomeid = Guid.NewGuid(),
                    Userid = userId,
                    Categoryid = category.Categoryid,
                    Title = dto.Title,
                    Description = dto.Description,
                    Amount = dto.Amount,
                    Date = dto.Date,
                    Source = dto.Source,
                    Createdat = DateTime.UtcNow,
                    Isenabled = true,
                    Isdeleted = false
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

            response.Data = incomesToAdd.Select(e => e.Incomeid).ToList();
            return response;
        }

        public async Task UpdateIncomeAsync(Guid userId, UpdateIncomeDto dto)
        {
            var income = await _unitOfWork.Incomes.GetByIdAsync(dto.Incomeid)
                ?? throw new Exception("Income not found");

            if(income.Isdeleted == true)
            {
                throw new Exception("Income not found");
            }

            if (income.Userid != userId)
                throw new UnauthorizedAccessException();

            string formattedCategory = dto.Category.ToPascalCase();
            var category = await _unitOfWork.Categories.GetByNameAsync(formattedCategory);
            if (category == null)
            {
                category = new Category
                {
                    Categoryid = Guid.NewGuid(),
                    Name = formattedCategory,
                    Type = "Income",
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

            income.Categoryid = category.Categoryid;
            income.Source = dto.Source;
            income.Description = dto.Description;
            income.Amount = dto.Amount;
            income.Date = dto.Date;
            income.Updatedat = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteIncomeAsync(Guid userId, Guid incomeId)
        {
            var income = await _unitOfWork.Incomes.GetByIdAsync(incomeId)
                ?? throw new Exception("Income not found");

            if (income.Userid != userId)
                throw new UnauthorizedAccessException();

            income.Isdeleted = true;
            income.Isenabled = false;
            income.Updatedat = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<TableResponseDto<IncomeTableDto>> GetUserIncomesAsync(Guid userId, TableRequestDto requestDto)
        {
            var incomes = _filterService.GetFilteredIncomes(_unitOfWork.Incomes.AsQueryable(), userId, requestDto.DashboardFilter)
                          .Select(e => new IncomeTableDto
                          {
                              Amount = e.Amount,
                              Date = e.Date,
                              Category = e.Category.Name,
                              Description = e.Description,
                              Source = e.Source,
                              Title = e.Title
                          }); ;

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
