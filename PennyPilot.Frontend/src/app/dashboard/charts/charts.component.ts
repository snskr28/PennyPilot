import { Component, inject, OnInit } from '@angular/core';
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
  private chartsService = inject(ChartsService);
  expCategoriesLoading = true;

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
        const expenseCategories = res.data?.ExpenseCategories;
        if (res.success && expenseCategories) {
          this.expenseCategoriesPieChart = {
            labels: Object.keys(expenseCategories),
            datasets: [{ data: Object.values(expenseCategories) }],
          };
        } 
        this.expCategoriesLoading = false;
      },
      error:() => {
        this.expCategoriesLoading = false;
      },
    });
  }
}
