using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PennyPilot.Backend.Application.DTOs;
using PennyPilot.Backend.Domain.Entities;

namespace PennyPilot.Backend.Application.Interfaces
{
    public interface IFilterService
    {
        IQueryable<Expense> GetFilteredExpenses(IQueryable<Expense> expenses, Guid userId, DashboardFilterDto dashboardFilter);
        IQueryable<Income> GetFilteredIncomes(IQueryable<Income> incomes, Guid userId, DashboardFilterDto dashboardFilter);
    }
}
