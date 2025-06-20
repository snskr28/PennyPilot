import { Component, inject, OnInit, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MATERIAL_IMPORTS } from '../../shared/material';
import { ChartConfiguration, ChartType } from 'chart.js';
import { BaseChartDirective } from 'ng2-charts';
import { ChartsService } from '../services/charts.service';
import { ApiResponse } from '../../shared/api-response.model';
import { PieChartsResponse } from '../models/pie-charts-response.model';

@Component({
  selector: 'app-dashboard-charts',
  standalone: true,
  imports: [CommonModule, BaseChartDirective, ...MATERIAL_IMPORTS],
  templateUrl: './charts.component.html',
  styleUrls: ['./charts.component.scss'],
})
export class ChartsComponent implements OnInit {
  @ViewChild(BaseChartDirective) chart?: BaseChartDirective;

  private chartsService = inject(ChartsService);
  expCategoriesLoading = true;
  expCategoriesError: string | null = null;

  private pieColors = [
  "#FF6384", // Soft Red/Pink
  "#FF9F40", // Orange
  "#FFCD56", // Yellow-Orange
  "#4BC0C0", // Teal
  "#36A2EB", // Blue
  "#9966FF", // Purple
  "#C9CBCF", // Light Grey
  "#FF6F91", // Pink
  "#FF8C42", // Orange
  "#F9F871", // Lemon
  "#A1DE93", // Mint Green
  "#62B6CB", // Sky Blue
  "#845EC2", // Violet
  "#FFC75F", // Gold
  "#F67280", // Coral Pink
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

  // Pie Chart Configuration
  expenseCategoriesPieChart: ChartConfiguration<'pie'>['data'] = {
    labels: [],
    datasets: [
      {
        data: [],
      },
    ],
  };

  pieChartOptions: ChartConfiguration<'pie'>['options'] = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: { position: 'right' },
    },
  };

  ngOnInit(): void {
    this.chartsService.getPieChartsData().subscribe({
      next: (res: ApiResponse<PieChartsResponse>) => {
        const expenseCategories = res.data?.expenseCategories;
        if (res.success && expenseCategories) {
          let labels = Object.keys(expenseCategories);
          let data = Object.values(expenseCategories);

          this.expenseCategoriesPieChart = {
            labels: [...labels],
            datasets: [{ 
              data: [...data],
              backgroundColor: this.pieColors.slice(0, labels.length),
            }],
          };

          setTimeout(() => this.chart?.update(), 0);

          this.expCategoriesError = null;
        } else {
          this.expenseCategoriesPieChart = {
            labels: [],
            datasets: [{ data: [] }],
          };
          this.expCategoriesError = 'No expense category data available.';
        }
        this.expCategoriesLoading = false;
      },
      error: () => {
        this.expenseCategoriesPieChart = {
          labels: [],
          datasets: [{ data: [] }],
        };
        this.expCategoriesError = 'Failed to load expense categories.';
        this.expCategoriesLoading = false;
      },
    });
  }
}
