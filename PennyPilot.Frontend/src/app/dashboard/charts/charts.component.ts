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

type DonutFilterKey =
  | 'expenseCategory'
  | 'userExpense'
  | 'incomeCategory'
  | 'incomeSource';

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
  @ViewChild('expenseCategoriesCanvas')
  expenseCategoriesCanvas?: BaseChartDirective;
  @ViewChild('userExpensesCanvas') userExpensesCanvas?: BaseChartDirective;
  @ViewChild('incomeCategoriesCanvas')
  incomeCategoriesCanvas?: BaseChartDirective;
  @ViewChild('incomeSourcesCanvas') incomeSourcesCanvas?: BaseChartDirective;

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

  selectedSegments: Record<DonutFilterKey, string | null> = {
    expenseCategory: null,
    userExpense: null,
    incomeCategory: null,
    incomeSource: null,
  };

  // Store original color mappings to maintain colors when filtering
  private originalColorMappings: Record<DonutFilterKey, Map<string, string>> = {
    expenseCategory: new Map(),
    userExpense: new Map(),
    incomeCategory: new Map(),
    incomeSource: new Map(),
  };

  // Different color palettes for each donut chart
  private expenseCategoriesColors = [
    '#FF6B6B', // Coral Red
    '#FF8E53', // Orange
    '#FF6B9D', // Pink
    '#C44569', // Dark Pink
    '#F8B500', // Golden Yellow
    '#FF4757', // Red
    '#FF3838', // Bright Red
    '#FF6348', // Tomato
    '#FF7675', // Light Red
    '#FD79A8', // Pink
    '#E84393', // Magenta
    '#A29BFE', // Light Purple
    '#6C5CE7', // Purple
    '#74B9FF', // Sky Blue
    '#0984E3', // Blue
  ];

  private userExpensesColors = [
    '#00D2D3', // Turquoise
    '#55A3FF', // Light Blue
    '#6C5CE7', // Purple
    '#74B9FF', // Sky Blue
    '#0984E3', // Blue
    '#A29BFE', // Light Purple
    '#00B894', // Mint
    '#00A085', // Dark Teal
    '#26DE81', // Green
    '#2ECC71', // Emerald
    '#00A8FF', // Dodger Blue
    '#0097E6', // Blue
    '#8C7AE6', // Soft Purple
    '#7B68EE', // Medium Slate Blue
  ];

  private incomeCategoriesColors = [
    '#26DE81', // Bright Green
    '#00A085', // Dark Teal
    '#55A3FF', // Light Blue
    '#74B9FF', // Sky Blue
    '#0984E3', // Blue
    '#00CEC9', // Teal
    '#00D2D3', // Turquoise
    '#1DD1A1', // Sea Green
    '#10AC84', // Green
    '#00A8FF', // Dodger Blue
    '#0097E6', // Blue
    '#8C7AE6', // Soft Purple
    '#7B68EE', // Medium Slate Blue
  ];

  private incomeSourcesColors = [
    '#8854D0', // Purple
    '#F0932B', // Orange
    '#EB4D4B', // Red
    '#6AB04C', // Green
    '#22A6B3', // Teal
    '#3867D6', // Blue
    '#F79F1F', // Amber
    '#EE5A52', // Coral
    '#5F9EA0', // Cadet Blue
    '#40739E', // Steel Blue
    '#487EB0', // Air Force Blue
    '#8C7AE6', // Soft Purple
    '#00A8FF', // Dodger Blue
    '#9C88FF', // Light Purple
    '#FF9FF3', // Pink
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

  private getColorsForLabels(labels: string[], filterKey: DonutFilterKey, colorPalette: string[]): string[] {
    const colors: string[] = [];
    
    labels.forEach(label => {
      if (this.originalColorMappings[filterKey].has(label)) {
        // Use the original color if we have it mapped
        colors.push(this.originalColorMappings[filterKey].get(label)!);
      } else {
        // Assign a new color and store the mapping
        const colorIndex = this.originalColorMappings[filterKey].size % colorPalette.length;
        const newColor = colorPalette[colorIndex];
        this.originalColorMappings[filterKey].set(label, newColor);
        colors.push(newColor);
      }
    });
    
    return colors;
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
            const labels = Object.keys(expenseCategories);
            this.expenseCategoriesDonutChart = {
              labels: labels,
              datasets: [
                {
                  data: Object.values(expenseCategories),
                  backgroundColor: this.getColorsForLabels(labels, 'expenseCategory', this.expenseCategoriesColors),
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
            const labels = Object.keys(userExpenses);
            this.userExpensesDonutChart = {
              labels: labels,
              datasets: [
                {
                  data: Object.values(userExpenses),
                  backgroundColor: this.getColorsForLabels(labels, 'userExpense', this.userExpensesColors),
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
            this.userExpensesError = 'No user expenses data available.';
          }

          if (incomeCategories && Object.keys(incomeCategories).length > 0) {
            const labels = Object.keys(incomeCategories);
            this.incomeCategoriesDonutChart = {
              labels: labels,
              datasets: [
                {
                  data: Object.values(incomeCategories),
                  backgroundColor: this.getColorsForLabels(labels, 'incomeCategory', this.incomeCategoriesColors),
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
            const labels = Object.keys(incomeSources);
            this.incomeSourcesDonutChart = {
              labels: labels,
              datasets: [
                {
                  data: Object.values(incomeSources),
                  backgroundColor: this.getColorsForLabels(labels, 'incomeSource', this.incomeSourcesColors),
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
            this.incomeSourcesError = 'No income sources data available.';
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
              backgroundColor:
                i === 0 ? 'rgba(255, 99, 132, 0.4)' : 'rgba(54, 162, 235, 0.4)', // Expenses, Income
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

  onDonutSegmentClick(
    filterKey: DonutFilterKey,
    event: any
  ): void {
    const activePoints = event.active;

    if (!activePoints?.length) return;

    const chart = activePoints[0].element.$context.chart;
    const index = activePoints[0].index;
    const label = chart.data.labels?.[index];

    if (!label || typeof label !== 'string') return;

    // Toggle logic
    if (this.selectedSegments[filterKey] === label) {
      this.selectedSegments[filterKey] = null;
      this.dashboardFilter[filterKey] = null;
    } else {
      this.selectedSegments[filterKey] = label;
      this.dashboardFilter[filterKey] = label;
    }

    this.reloadCharts(this.dashboardFilter);
  }
}