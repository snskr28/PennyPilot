import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MATERIAL_IMPORTS } from '../../shared/material';
import { ChartConfiguration, ChartType } from 'chart.js';
import { BaseChartDirective } from 'ng2-charts';
 
@Component({
  selector: 'app-dashboard-charts',
  standalone: true,
  imports: [CommonModule, BaseChartDirective,...MATERIAL_IMPORTS],
  templateUrl: './charts.component.html',
  styleUrls: ['./charts.component.scss']
})
export class ChartsComponent implements OnInit {
  // Bar Chart Configuration
  barChartData: ChartConfiguration<'bar'>['data'] = {
    labels: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun'],
    datasets: [
      { data: [2100, 1800, 2400, 1900, 2600, 2200], label: 'Income' },
      { data: [1500, 1300, 1900, 1600, 2000, 1800], label: 'Expenses' }
    ]
  };

  barChartOptions: ChartConfiguration<'bar'>['options'] = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: { position: 'top' }
    }
  };

  // Pie Chart Configuration
  pieChartData: ChartConfiguration<'pie'>['data'] = {
    labels: ['Groceries', 'Rent', 'Utilities', 'Entertainment', 'Transport'],
    datasets: [{
      data: [300, 1200, 250, 200, 150]
    }]
  };

  pieChartOptions: ChartConfiguration<'pie'>['options'] = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: { position: 'right' }
    }
  };

  ngOnInit(): void {
    // We'll fetch real data here later
  }
}