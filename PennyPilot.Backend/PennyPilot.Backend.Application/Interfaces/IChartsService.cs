using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PennyPilot.Backend.Application.DTOs;

namespace PennyPilot.Backend.Application.Interfaces
{
    public interface IChartsService
    {
        Task<DonutChartsDto> GetDonutChartsData(Guid userId, DashboardFilterDto dashboardFilter);
        Task<BarChartResponseDto> GetIncomeExpenseBarChartData(Guid userId, DashboardFilterDto dashboardFilter);
        Task<LineChartResponseDto> GetIncomeExpenseLineChartData(Guid userId, DashboardFilterDto dashboardFilter);
    }
}
