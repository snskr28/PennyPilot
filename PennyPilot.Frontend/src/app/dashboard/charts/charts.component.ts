import { Component, inject, Input, OnChanges, OnInit, ViewChild, SimpleChange, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MATERIAL_IMPORTS } from '../../shared/material';
import { ChartConfiguration, ChartType } from 'chart.js';
import { BaseChartDirective } from 'ng2-charts';
import { ChartsService } from '../services/charts.service';
import { ApiResponse } from '../../shared/api-response.model';
import { DonutChartsResponse } from '../models/donut-charts-response.model';
import { DashboardFilter } from '../models/dashboard-filter.model';

@Component({
  selector: 'app-dashboard-charts',
  standalone: true,
  imports: [CommonModule, BaseChartDirective, ...MATERIAL_IMPORTS],
  templateUrl: './charts.component.html',
  styleUrls: ['./charts.component.scss'],
})
export class ChartsComponent implements OnInit, OnChanges {
  @Input() dashboardFilter!: DashboardFilter;

  @ViewChild(BaseChartDirective) chart?: BaseChartDirective;

  private chartsService = inject(ChartsService);
  expCategoriesLoading = true;
  expCategoriesError: string | null = null;
  userExpensesLoading = true;
  userExpensesError: string | null = null;
  incomeCategoriesLoading = true;
  incomeCategoriesError: string | null = null;
  incomeSourcesLoading = true;
  incomeSourcesError: string | null = null;

  private pieColors = [
    '#FF6384', // Soft Red/Pink
    '#FF9F40', // Orange
    '#FFCD56', // Yellow-Orange
    '#4BC0C0', // Teal
    '#36A2EB', // Blue
    '#9966FF', // Purple
    '#C9CBCF', // Light Grey
    '#FF6F91', // Pink
    '#FF8C42', // Orange
    '#F9F871', // Lemon
    '#A1DE93', // Mint Green
    '#62B6CB', // Sky Blue
    '#845EC2', // Violet
    '#FFC75F', // Gold
    '#F67280', // Coral Pink
  ];

  // Bar Chart Configuration
  barChartData: ChartConfiguration<'bar'>['data'] = {
    labels: [
      'Jan',
      'Feb',
      'Mar',
      'Apr',
      'May',
      'Jun',
      'Jul',
      'Aug',
      'Sep',
      'Oct',
      'Nov',
      'Dec',
    ],
    datasets: [
      {
        data: [
          2100, 1800, 2400, 1900, 2600, 2200, 1000, 2500, 5900, 2800, 2500,
          3000,
        ],
        label: 'Income',
      },
      {
        data: [
          1500, 1300, 1900, 1600, 2000, 1800, 1200, 1800, 1700, 2200, 1100,
          2500,
        ],
        label: 'Expenses',
      },
    ],
  };

  barChartOptions: ChartConfiguration<'bar'>['options'] = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: { position: 'top' },
    },
  };

  // Donut Chart Configuration
  expenseCategoriesDonutChart: ChartConfiguration<'doughnut'>['data'] = {
    labels: [],
    datasets: [
      {
        data: [],
      },
    ],
  };

  userExpensesDonutChart: ChartConfiguration<'doughnut'>['data'] = {
    labels: [],
    datasets: [
      {
        data: [],
      },
    ],
  };

  incomeCategoriesDonutChart: ChartConfiguration<'doughnut'>['data'] = {
    labels: [],
    datasets: [
      {
        data: [],
      },
    ],
  };

  incomeSourcesDonutChart: ChartConfiguration<'doughnut'>['data'] = {
    labels: [],
    datasets: [
      {
        data: [],
      },
    ],
  };

  donutChartOptions: ChartConfiguration<'doughnut'>['options'] = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: { position: 'right' },
    },
  };

  ngOnInit(): void {
    this.reloadCharts(this.dashboardFilter);
  }

  ngOnChanges(changes: SimpleChanges): void {
    if(changes['filter']) {
      this.reloadCharts(this.dashboardFilter);
    }
  }

  reloadCharts(filter: DashboardFilter) {
    this.expCategoriesLoading = true;
    this.userExpensesLoading = true;
    this.incomeCategoriesLoading = true;
    this.incomeSourcesLoading = true;
    // Call your service with the filter
    this.chartsService.getDonutChartsData(filter).subscribe({
      next: (res: ApiResponse<DonutChartsResponse>) => {
        const expenseCategories = res.data?.expenseCategories;
        const userExpenses = res.data?.userExpenses;
        const incomeCategories = res.data?.incomeCategories;
        const incomeSources = res.data?.incomeSources;
        if (res.success) {
          if (expenseCategories && Object.keys(expenseCategories).length > 0) {
            this.expenseCategoriesDonutChart = {
              labels: Object.keys(expenseCategories),
              datasets: [
                {
                  data: Object.values(expenseCategories),
                  backgroundColor: this.pieColors.slice(
                    0,
                    Object.keys(expenseCategories).length
                  ),
                  hoverOffset: 8,
                },
              ],
            };

            this.expCategoriesError = null;
          } else {
            this.expenseCategoriesDonutChart = {
              labels: [],
              datasets: [{ data: [] }],
            };
            this.expCategoriesError = 'No expense category data available.';
          }

          if (userExpenses && Object.keys(userExpenses).length > 0) {
            this.userExpensesDonutChart = {
              labels: Object.keys(userExpenses),
              datasets: [
                {
                  data: Object.values(userExpenses),
                  hoverOffset: 8,
                },
              ],
            };

            this.userExpensesError = null;
          } else {
            this.userExpensesDonutChart = {
              labels: [],
              datasets: [{ data: [] }],
            };
            this.userExpensesError = 'No income category data available.';
          }

          if (incomeCategories && Object.keys(incomeCategories).length > 0) {
            this.incomeCategoriesDonutChart = {
              labels: Object.keys(incomeCategories),
              datasets: [
                {
                  data: Object.values(incomeCategories),
                  hoverOffset: 8,
                },
              ],
            };

            this.incomeCategoriesError = null;
          } else {
            this.incomeCategoriesDonutChart = {
              labels: [],
              datasets: [{ data: [] }],
            };
            this.incomeCategoriesError = 'No income category data available.';
          }

          if (incomeSources && Object.keys(incomeSources).length > 0) {
            this.incomeSourcesDonutChart = {
              labels: Object.keys(incomeSources),
              datasets: [
                {
                  data: Object.values(incomeSources),
                  hoverOffset: 8,
                },
              ],
            };

            this.incomeSourcesError = null;
          } else {
            this.incomeSourcesDonutChart = {
              labels: [],
              datasets: [{ data: [] }],
            };
            this.incomeSourcesError = 'No income category data available.';
          }
        }
        this.expCategoriesLoading = false;
        this.userExpensesLoading = false;
        this.incomeCategoriesLoading = false;
        this.incomeSourcesLoading = false;
      },
      error: () => {
        this.expenseCategoriesDonutChart = {
          labels: [],
          datasets: [{ data: [] }],
        };
        this.expCategoriesError = 'Failed to load Expense Categories.';
        this.expCategoriesLoading = false;

        this.userExpensesDonutChart = {
          labels: [],
          datasets: [{ data: [] }],
        };
        this.userExpensesError = 'Failed to load User Expenses.';
        this.userExpensesLoading = false;

        this.incomeCategoriesDonutChart = {
          labels: [],
          datasets: [{ data: [] }],
        };
        this.incomeCategoriesError = 'Failed to load Income Categories.';
        this.incomeCategoriesLoading = false;

        this.incomeSourcesDonutChart = {
          labels: [],
          datasets: [{ data: [] }],
        };
        this.incomeSourcesError = 'Failed to load Income Sources.';
        this.incomeSourcesLoading = false;
      },
    });
  }
}
