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

        public ChartsService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<DonutChartsDtoModel> GetDonutChartsData(Guid userId)
        {
            var expenseCategories = _unitOfWork.Expenses.AsQueryable()
                                    .Where(e => e.UserId == userId && !e.IsDeleted && e.IsEnabled)
                                    .GroupBy(x => x.Category.Name)
                                    .ToDictionary(x => x.Key, x => x.Count());

            var userExpenses = _unitOfWork.Expenses.AsQueryable()
                               .Where(e => e.UserId == userId && !e.IsDeleted && e.IsEnabled)
                               .GroupBy(x => x.PaidBy)
                               .ToDictionary(x => x.Key, x => x.Sum(y=>y.Amount));

            var incomeCategories = _unitOfWork.Incomes.AsQueryable()
                                   .Where(i => i.UserId == userId && !i.IsDeleted && i.IsEnabled)
                                   .GroupBy(x => x.Category.Name)
                                   .ToDictionary(x => x.Key, x => x.Count());

            var incomeSources = _unitOfWork.Incomes.AsQueryable()
                                .Where(i => i.UserId == userId && !i.IsDeleted && i.IsEnabled)
                                .GroupBy(x => x.Source)
                                .ToDictionary(x => x.Key, x => x.Sum(y=>y.Amount)); ;

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
