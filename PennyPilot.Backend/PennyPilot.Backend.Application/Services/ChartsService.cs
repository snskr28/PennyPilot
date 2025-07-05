using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PennyPilot.Backend.Application.Constants;
using PennyPilot.Backend.Application.DTOs;
using PennyPilot.Backend.Application.Interfaces;
using PennyPilot.Backend.Domain.Entities;
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

        public async Task<BarChartResponseDto> GetIncomeExpenseBarChartData(Guid userId, DashboardFilterDto dashboardFilter)
        {
            var expenses = _filterService.GetFilteredExpenses(_unitOfWork.Expenses.AsQueryable(), userId, dashboardFilter);
            var incomes = _filterService.GetFilteredIncomes(_unitOfWork.Incomes.AsQueryable(), userId, dashboardFilter);

            // Get date ranges for both expenses and incomes
            var expenseData = expenses.ToList();
            var incomeData = incomes.ToList();

            // Determine the date range
            var allDates = new List<DateTime>();
            if (expenseData.Any()) allDates.AddRange(expenseData.Select(e => e.Date));
            if (incomeData.Any()) allDates.AddRange(incomeData.Select(i => i.Date));

            if (!allDates.Any())
            {
                // No data available
                return new BarChartResponseDto
                {
                    Labels = new List<string>(),
                    Datasets = new List<BarChartDatasetDto>
                    {
                        new BarChartDatasetDto { Label = "Expenses", Data = new List<decimal>() },
                        new BarChartDatasetDto { Label = "Income", Data = new List<decimal>() }
                    }
                };
            }

            var minDate = allDates.Min();
            var maxDate = allDates.Max();

            // Generate labels and group data based on granularity
            var (labels, expenseValues, incomeValues) = GenerateChartData(expenseData, incomeData, minDate, maxDate, dashboardFilter.Granularity);

            return new BarChartResponseDto
            {
                Labels = labels,
                Datasets = new List<BarChartDatasetDto>
                {
                    new BarChartDatasetDto
                    {
                        Label = "Expenses",
                        Data = expenseValues
                    },
                    new BarChartDatasetDto
                    {
                        Label = "Income",
                        Data = incomeValues
                    }
                }
            };
        }

        private (List<string> labels, List<decimal> expenseValues, List<decimal> incomeValues) GenerateChartData(List<Expense> expenses, List<Income> incomes, DateTime minDate, DateTime maxDate, string granularity)
        {
            var labels = new List<string>();
            var expenseValues = new List<decimal>();
            var incomeValues = new List<decimal>();

            switch (granularity.ToLower())
            {
                case GranularityConstant.Daily:
                    GenerateDailyData(expenses, incomes, minDate, maxDate, labels, expenseValues, incomeValues);
                    break;
                case GranularityConstant.Monthly:
                    GenerateMonthlyData(expenses, incomes, minDate, maxDate, labels, expenseValues, incomeValues);
                    break;
                case GranularityConstant.Quarterly:
                    GenerateQuarterlyData(expenses, incomes, minDate, maxDate, labels, expenseValues, incomeValues);
                    break;
                case GranularityConstant.Yearly:
                    GenerateYearlyData(expenses, incomes, minDate, maxDate, labels, expenseValues, incomeValues);
                    break;
                default:
                    GenerateYearlyData(expenses, incomes, minDate, maxDate, labels, expenseValues, incomeValues);
                    break;
            }

            return (labels, expenseValues, incomeValues);
        }

        private void GenerateDailyData(List<Expense> expenses, List<Income> incomes, DateTime minDate, DateTime maxDate,
            List<string> labels, List<decimal> expenseValues, List<decimal> incomeValues)
        {
            var expenseDict = expenses.GroupBy(e => e.Date.Date)
                .ToDictionary(g => g.Key, g => g.Sum(e => e.Amount));

            var incomeDict = incomes.GroupBy(i => i.Date.Date)
                .ToDictionary(g => g.Key, g => g.Sum(i => i.Amount));

            for (var date = minDate.Date; date <= maxDate.Date; date = date.AddDays(1))
            {
                labels.Add(date.ToString("yyyy-MM-dd"));
                expenseValues.Add(expenseDict.GetValueOrDefault(date, 0));
                incomeValues.Add(incomeDict.GetValueOrDefault(date, 0));
            }
        }

        private void GenerateMonthlyData(List<Expense> expenses, List<Income> incomes, DateTime minDate, DateTime maxDate,
            List<string> labels, List<decimal> expenseValues, List<decimal> incomeValues)
        {
            var expenseDict = expenses.GroupBy(e => new { e.Date.Year, e.Date.Month })
                .ToDictionary(g => g.Key, g => g.Sum(e => e.Amount));

            var incomeDict = incomes.GroupBy(i => new { i.Date.Year, i.Date.Month })
                .ToDictionary(g => g.Key, g => g.Sum(i => i.Amount));

            var startMonth = new DateTime(minDate.Year, minDate.Month, 1);
            var endMonth = new DateTime(maxDate.Year, maxDate.Month, 1);

            for (var month = startMonth; month <= endMonth; month = month.AddMonths(1))
            {
                var key = new { Year = month.Year, Month = month.Month };
                labels.Add(month.ToString("yyyy-MM"));
                expenseValues.Add(expenseDict.GetValueOrDefault(key, 0));
                incomeValues.Add(incomeDict.GetValueOrDefault(key, 0));
            }
        }

        private void GenerateQuarterlyData(List<Expense> expenses, List<Income> incomes, DateTime minDate, DateTime maxDate,
            List<string> labels, List<decimal> expenseValues, List<decimal> incomeValues)
        {
            var expenseDict = expenses.GroupBy(e => new { e.Date.Year, Quarter = GetQuarter(e.Date) })
                .ToDictionary(g => g.Key, g => g.Sum(e => e.Amount));

            var incomeDict = incomes.GroupBy(i => new { i.Date.Year, Quarter = GetQuarter(i.Date) })
                .ToDictionary(g => g.Key, g => g.Sum(i => i.Amount));

            var startQuarter = GetQuarter(minDate);
            var endQuarter = GetQuarter(maxDate);

            for (var year = minDate.Year; year <= maxDate.Year; year++)
            {
                var startQ = (year == minDate.Year) ? startQuarter : 1;
                var endQ = (year == maxDate.Year) ? endQuarter : 4;

                for (var quarter = startQ; quarter <= endQ; quarter++)
                {
                    var key = new { Year = year, Quarter = quarter };
                    labels.Add($"{year}-Q{quarter}");
                    expenseValues.Add(expenseDict.GetValueOrDefault(key, 0));
                    incomeValues.Add(incomeDict.GetValueOrDefault(key, 0));
                }
            }
        }

        private void GenerateYearlyData(List<Expense> expenses, List<Income> incomes, DateTime minDate, DateTime maxDate,
            List<string> labels, List<decimal> expenseValues, List<decimal> incomeValues)
        {
            var expenseDict = expenses.GroupBy(e => e.Date.Year)
                .ToDictionary(g => g.Key, g => g.Sum(e => e.Amount));

            var incomeDict = incomes.GroupBy(i => i.Date.Year)
                .ToDictionary(g => g.Key, g => g.Sum(i => i.Amount));

            for (var year = minDate.Year; year <= maxDate.Year; year++)
            {
                labels.Add(year.ToString());
                expenseValues.Add(expenseDict.GetValueOrDefault(year, 0));
                incomeValues.Add(incomeDict.GetValueOrDefault(year, 0));
            }
        }

        private int GetQuarter(DateTime date)
        {
            return (date.Month - 1) / 3 + 1;
        }
    }
}
