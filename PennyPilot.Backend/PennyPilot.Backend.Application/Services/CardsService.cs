using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PennyPilot.Backend.Application.Constants;
using PennyPilot.Backend.Application.DTOs;
using PennyPilot.Backend.Application.Interfaces;
using PennyPilot.Backend.Domain.Interfaces;

namespace PennyPilot.Backend.Application.Services
{
    public class CardsService : ICardsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFilterService _filterService;

        public CardsService(IUnitOfWork unitOfWork, IFilterService filterService)
        {
            _unitOfWork = unitOfWork;
            _filterService = filterService;
        }

        public async Task<SummaryCardsResponseDto> GetSummaryCards(Guid userId, DashboardFilterDto dashboardFilter)
        {
            var expenses = _filterService.GetFilteredExpenses(_unitOfWork.Expenses.AsQueryable(), userId, dashboardFilter);
            var incomes = _filterService.GetFilteredIncomes(_unitOfWork.Incomes.AsQueryable(), userId, dashboardFilter);

            decimal totalIncome = 0.00m;
            decimal totalExpenses = 0.00m;

            if (expenses.Any())
            {
                totalExpenses = expenses.Sum(x => x.Amount);
            }
            if (incomes.Any())
            {
                totalIncome = incomes.Sum(x => x.Amount);
            }

            var netSavings = totalIncome - totalExpenses;

            return new SummaryCardsResponseDto
            {
                TotalIncome = new CardDto { Name = CardsNameConstant.TotalIncome, Value = totalIncome },
                TotalExpenses = new CardDto { Name = CardsNameConstant.TotalExpenses, Value = totalExpenses },
                NetSavings = new CardDto { Name = CardsNameConstant.NetSavings, Value = netSavings }
            };
        }
    }
}
