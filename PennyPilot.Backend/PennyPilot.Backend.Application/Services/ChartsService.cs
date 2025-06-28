using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PennyPilot.Backend.Application.DTOs;
using PennyPilot.Backend.Application.Interfaces;
using PennyPilot.Backend.Domain.Interfaces;

namespace PennyPilot.Backend.Application.Services
{
    public class ChartsService : IChartsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFilterService _filterService;

        public ChartsService(IUnitOfWork unitOfWork, IFilterService filterService)
        {
            _unitOfWork = unitOfWork;
            _filterService = filterService;
        }

        public async Task<DonutChartsDtoModel> GetDonutChartsData(Guid userId, DashboardFilterDto dashboardFilter)
        {
            var expenses = _filterService.GetFilteredExpenses(_unitOfWork.Expenses.AsQueryable(), userId, dashboardFilter);
            var incomes = _filterService.GetFilteredIncomes(_unitOfWork.Incomes.AsQueryable(), userId, dashboardFilter);

            var expenseCategories = expenses.GroupBy(x => x.Category.Name).ToDictionary(x => x.Key, x => x.Count());

            var userExpenses = expenses.GroupBy(x => x.PaidBy).ToDictionary(x => x.Key, x => x.Sum(y=>y.Amount));

            var incomeCategories = incomes.GroupBy(x => x.Category.Name).ToDictionary(x => x.Key, x => x.Count());

            var incomeSources =  incomes.GroupBy(x => x.Source).ToDictionary(x => x.Key, x => x.Sum(y=>y.Amount));

            return new DonutChartsDtoModel
            {
                ExpenseCategories = expenseCategories,
                UserExpenses = userExpenses,
                IncomeCategories = incomeCategories,
                IncomeSources = incomeSources
            };
        }
    }
}
