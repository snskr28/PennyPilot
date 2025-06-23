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

            return new DonutChartsDtoModel
            {
                ExpenseCategories = expenseCategories
            };
        }
    }
}
