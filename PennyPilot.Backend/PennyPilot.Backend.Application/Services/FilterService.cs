using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PennyPilot.Backend.Application.DTOs;
using PennyPilot.Backend.Application.Interfaces;
using PennyPilot.Backend.Domain.Entities;

namespace PennyPilot.Backend.Application.Services
{
    public class FilterService : IFilterService
    {
        public IQueryable<Expense> GetFilteredExpenses(IQueryable<Expense> expenses, Guid userId, DashboardFilterDto dashboardFilter)
        {
            //Base Filter
            expenses = expenses.Where(e => e.Userid == userId && !e.Isdeleted && e.Isenabled);

            //Date Range Filters
            if (dashboardFilter.StartDate.HasValue)
                expenses = expenses.Where(e => e.Date >= dashboardFilter.StartDate.Value);

            if (dashboardFilter.EndDate.HasValue)
                expenses = expenses.Where(e => e.Date <= dashboardFilter.EndDate.Value);

            //Donut Chart Filters
            if (!string.IsNullOrWhiteSpace(dashboardFilter.ExpenseCategory))
                expenses = expenses.Where(e => e.Category.Name == dashboardFilter.ExpenseCategory);

            if (!string.IsNullOrWhiteSpace(dashboardFilter.UserExpense))
                expenses = expenses.Where(e => e.Paidby == dashboardFilter.UserExpense);

            return expenses;
        }

        public IQueryable<Income> GetFilteredIncomes(IQueryable<Income> incomes, Guid userId, DashboardFilterDto dashboardFilter)
        {
            //Base Filter
            incomes = incomes.Where(e => e.Userid == userId && !e.Isdeleted && e.Isenabled);

            //Date Range Filters
            if (dashboardFilter.StartDate.HasValue)
                incomes = incomes.Where(e => e.Date >= dashboardFilter.StartDate.Value);

            if (dashboardFilter.EndDate.HasValue)
                incomes = incomes.Where(e => e.Date <= dashboardFilter.EndDate.Value);

            //Donut Chart Filters
            if (!string.IsNullOrWhiteSpace(dashboardFilter.IncomeCategory))
                incomes = incomes.Where(e => e.Category.Name == dashboardFilter.IncomeCategory);

            if (!string.IsNullOrWhiteSpace(dashboardFilter.IncomeSource))
                incomes = incomes.Where(e => e.Source == dashboardFilter.IncomeSource);

            return incomes;
        }
    }
}
