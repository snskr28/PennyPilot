import { Component, ViewChild } from '@angular/core';
import { MATERIAL_IMPORTS } from '../shared/material';
import { CommonModule } from '@angular/common';
import { ChartsComponent } from './charts/charts.component';
import { TransactionsTableComponent } from './transactions-table/transactions-table.component';
import { AuthService } from '../auth/services/auth.service';
import { TimeRangeFilterComponent } from './time-range-filter/time-range-filter.component';
import { DashboardFilter } from './models/dashboard-filter.model';
import { Chart } from 'chart.js';

@Component({
  selector: 'app-dashboard',
  imports: [
    CommonModule,
    ChartsComponent,
    TransactionsTableComponent,
    TimeRangeFilterComponent,
    ...MATERIAL_IMPORTS,
  ],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss',
})
export class DashboardComponent {
  dashboardFilter: DashboardFilter = {
    startDate: null,
    endDate: null,
    expenseCategory: null,
    incomeCategory: null,
    userExpense: null,
    incomeSource: null,
  };

  @ViewChild(ChartsComponent) chartsComp!: ChartsComponent;
  @ViewChild(TransactionsTableComponent) tableComp!: TransactionsTableComponent;

  summaryCards = [
    { title: 'Total Income', amount: 0, icon: 'trending_up', color: '#10B981' },
    {
      title: 'Total Expenses',
      amount: 0,
      icon: 'trending_down',
      color: '#EF4444',
    },
    { title: 'Net Savings', amount: 0, icon: 'savings', color: '#6366F1' },
  ];

  constructor(private authService: AuthService) {}

  ngOnInit(): void {
    // We'll fetch data here later
  }

  logout() {
    this.authService.logout();
  }

  onDateRangeChange(range: { start: Date | null; end: Date | null }) {
    this.dashboardFilter = { ...this.dashboardFilter, startDate: range.start, endDate: range.end };
    this.reloadAllWidgets();
  }

   reloadAllWidgets() {
    if (this.chartsComp) {
      this.chartsComp.reloadCharts(this.dashboardFilter);
    }
    if (this.tableComp) {
      this.tableComp.reloadTable(this.dashboardFilter);
    }
  }
}
