import { Component } from '@angular/core';
import { MATERIAL_IMPORTS } from '../shared/material';
import { CommonModule } from '@angular/common';
import { ChartsComponent } from './charts/charts.component';

@Component({
  selector: 'app-dashboard',
  imports: [CommonModule,ChartsComponent,...MATERIAL_IMPORTS],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent {
  summaryCards = [
    { title: 'Total Income', amount: 0, icon: 'trending_up', color: '#10B981' },
    { title: 'Total Expenses', amount: 0, icon: 'trending_down', color: '#EF4444' },
    { title: 'Net Savings', amount: 0, icon: 'savings', color: '#6366F1' }
  ];

  ngOnInit(): void {
    // We'll fetch data here later
  }
}
