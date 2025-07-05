import {
  Component,
  inject,
  Input,
  OnChanges,
  OnInit,
  ViewChild,
  SimpleChange,
  SimpleChanges,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { MATERIAL_IMPORTS } from '../../shared/material';
import { ChartConfiguration, ChartType } from 'chart.js';
import { BaseChartDirective } from 'ng2-charts';
import { ChartsService } from '../services/charts.service';
import { ApiResponse } from '../../shared/api-response.model';
import { DonutChartsResponse } from '../models/donut-charts-response.model';
import { DashboardFilter } from '../models/dashboard-filter.model';
import {
  Chart,
  CategoryScale,
  LinearScale,
  BarElement,
  Title,
  Tooltip,
  Legend,
} from 'chart.js';
import zoomPlugin from 'chartjs-plugin-zoom';

// Register Chart.js components
Chart.register(
  CategoryScale,
  LinearScale,
  BarElement,
  Title,
  Tooltip,
  Legend,
  zoomPlugin
);

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
  expenseIncomeBarChartLoading = true;
  expenseIncomeBarChartError: string | null = null;
  expenseIncomeLineChartLoading = true;
  expenseIncomeLineChartError: string | null = null;

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
    labels: [],
    datasets: [
      {
        data: [],
        label: 'Income',
      },
      {
        data: [],
        label: 'Expenses',
      },
    ],
  };

  barChartOptions: ChartConfiguration<'bar'>['options'] = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: { position: 'top' },
      zoom: {
        pan: {
          enabled: false,
          mode: 'x',
          threshold: 10,
        },
        limits: {
          x: { min: 0, max: 'original' },
        },
      },
    },
    scales: {
      x: {},
      y: {
        beginAtZero: true,
      },
    },
  };

  //Line Chart Configuration
   lineChartData: ChartConfiguration<'line'>['data'] = {
    labels: [],
    datasets: [
      {
        data: [],
        label: 'Income',
        fill: true,
        tension: 0.5,
      },
      {
        data: [],
        label: 'Expenses',
        fill: true,
        tension: 0.5,
      },
    ],
  };

  lineChartOptions: ChartConfiguration<'line'>['options'] = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: { position: 'top' },
      zoom: {
        pan: {
          enabled: false,
          mode: 'x',
          threshold: 10,
        },
        limits: {
          x: { min: 0, max: 'original' },
        },
      },
    },
    scales: {
      x: {},
      y: {
        beginAtZero: true,
      },
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
    if (changes['filter']) {
      this.reloadCharts(this.dashboardFilter);
    }
  }

  private configureBarChartOptions(dataLength: number): void {
    const shouldEnableScrolling = dataLength > 12;

    this.barChartOptions = {
      responsive: true,
      maintainAspectRatio: false,
      plugins: {
        legend: { position: 'top' },
        zoom: {
          pan: {
            enabled: shouldEnableScrolling,
            mode: 'x',
            threshold: 10,
          },
          limits: {
            x: {
              min: 0,
              max: shouldEnableScrolling ? dataLength - 1 : undefined,
            },
          },
        },
      },
      scales: {
        x: {
          min: shouldEnableScrolling ? 0 : undefined,
          max: shouldEnableScrolling ? 11 : undefined, // Show first 12 bars (0-11)
        },
        y: {
          beginAtZero: true,
        },
      },
    };
  }
  private configureLineChartOptions(dataLength: number): void {
    const shouldEnableScrolling = dataLength > 10;

    this.lineChartOptions = {
      responsive: true,
      maintainAspectRatio: false,
      plugins: {
        legend: { position: 'top' },
        zoom: {
          pan: {
            enabled: shouldEnableScrolling,
            mode: 'x',
            threshold: 8,
          },
          limits: {
            x: {
              min: 0,
              max: shouldEnableScrolling ? dataLength - 1 : undefined,
            },
          },
        },
      },
      scales: {
        x: {
          min: shouldEnableScrolling ? 0 : undefined,
          max: shouldEnableScrolling ? 9 : undefined, // Show first 12 bars (0-11)
        },
        y: {
          beginAtZero: true,
        },
      },
    };
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

    //Income and Expense Bar Chart and Line Chart
    this.expenseIncomeBarChartLoading = true;
    this.chartsService.getIncomeExpenseBarChartData(filter).subscribe({
      next: (res) => {
        if (
          res.success &&
          res.data?.labels.length > 0 &&
          res.data?.datasets.length > 0
        ) {
          this.configureBarChartOptions(res.data.labels.length);

          this.barChartData = {
            labels: res.data.labels,
            datasets: res.data.datasets.map((ds, i) => ({
              ...ds,
              backgroundColor: i === 0 ? '#FF6384' : '#36A2EB', // Expenses, Income
            })),
          };
          this.expenseIncomeBarChartError = null;
        } else {
          this.configureBarChartOptions(0);
          this.barChartData = {
            labels: [],
            datasets: [
              { data: [], label: 'Income', backgroundColor: '#36A2EB' },
              { data: [], label: 'Expenses', backgroundColor: '#FF6384' },
            ],
          };
          this.expenseIncomeBarChartError = 'No bar chart data available.';
        }
        this.expenseIncomeBarChartLoading = false;
      },
      error: () => {
        this.configureBarChartOptions(0);
        this.barChartData = {
          labels: [],
          datasets: [
            { data: [], label: 'Income', backgroundColor: '#36A2EB' },
            { data: [], label: 'Expenses', backgroundColor: '#FF6384' },
          ],
        };
        this.expenseIncomeBarChartError = 'Failed to load bar chart data.';
        this.expenseIncomeBarChartLoading = false;
      },
    });

    // Income and Expense Line Chart
    this.expenseIncomeLineChartLoading = true;
    this.chartsService.getIncomeExpenseLineChartData(filter).subscribe({
      next: (res) => {
        if (
          res.success &&
          res.data?.labels.length > 0 &&
          res.data?.datasets.length > 0
        ) {
          this.configureLineChartOptions(res.data.labels.length);

          this.lineChartData = {
            labels: res.data.labels,
            datasets: res.data.datasets.map((ds, i) => ({
              ...ds,
              backgroundColor: i === 0 ? 'rgba(255, 99, 132, 0.4)' : 'rgba(54, 162, 235, 0.4)', // Expenses, Income
              borderColor: i === 0 ? '#FF6384' : '#36A2EB', // Expenses, Income
              tension: 0.5,
              fill: true,
            })),
          };
          this.expenseIncomeLineChartError = null;
        } else {
          this.configureLineChartOptions(0);
          this.lineChartData = {
            labels: [],
            datasets: [
              { data: [], label: 'Income', backgroundColor: '#36A2EB' },
              { data: [], label: 'Expenses', backgroundColor: '#FF6384' },
            ],
          };
          this.expenseIncomeLineChartError = 'No line chart data available.';
        }
        this.expenseIncomeLineChartLoading = false;
      },
      error: () => {
        this.configureLineChartOptions(0);
        this.lineChartData = {
          labels: [],
          datasets: [
            { data: [], label: 'Income', backgroundColor: '#36A2EB' },
            { data: [], label: 'Expenses', backgroundColor: '#FF6384' },
          ],
        };
        this.expenseIncomeLineChartError = 'Failed to load line chart data.';
        this.expenseIncomeLineChartLoading = false;
      },
    });
  }
}
