import { Component, inject, ViewChild } from '@angular/core';
import { MATERIAL_IMPORTS } from '../shared/material';
import { CommonModule } from '@angular/common';
import { ChartsComponent } from './charts/charts.component';
import { TransactionsTableComponent } from './transactions-table/transactions-table.component';
import { AuthService } from '../auth/services/auth.service';
import { TimeRangeFilterComponent } from './time-range-filter/time-range-filter.component';
import { DashboardFilter } from './models/dashboard-filter.model';
import { Chart } from 'chart.js';
import { CardsService } from './services/cards.service';
import { ApiResponse } from '../shared/api-response.model';
import { SummaryCardsResponse } from './models/summary-cards-response.model';

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
    granularity: 'yearly',
    expenseCategory: null,
    incomeCategory: null,
    userExpense: null,
    incomeSource: null,
  };

  @ViewChild(ChartsComponent) chartsComp!: ChartsComponent;
  @ViewChild(TransactionsTableComponent) tableComp!: TransactionsTableComponent;

  private cardsService = inject(CardsService);
  summaryCardsLoading = true;
  summaryCardsError: string | null = null;

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

  getNetSavingsColor(amount: number): string {
    if (amount === 0) return '#111827'; // neutral color for zero savings
    return amount >= 0 ? 'rgb(25 153 30)' : 'rgb(221 27 27)'; // green or red
  }

  constructor(private authService: AuthService) {}

  ngOnInit(): void {
    this.reloadSummaryCards();
  }

  logout() {
    this.authService.logout();
  }

  reloadSummaryCards() {
    this.summaryCardsLoading = true;
    this.cardsService.getSummaryCardsData(this.dashboardFilter).subscribe({
      next: (res: ApiResponse<SummaryCardsResponse>) => {
        const totalIncome = res.data?.totalIncome?.value;
        const totalExpenses = res.data?.totalExpenses?.value;
        const netSavings = res.data?.netSavings?.value;

        if (res.success) {
          this.summaryCards = [
            {
              title: 'Total Income',
              amount: totalIncome || 0,
              icon: 'trending_up',
              color: '#10B981',
            },
            {
              title: 'Total Expenses',
              amount: totalExpenses || 0,
              icon: 'trending_down',
              color: '#EF4444',
            },
            {
              title: 'Net Savings',
              amount: netSavings || 0,
              icon: 'savings',
              color: '#6366F1',
            },
          ];
        }
      },
      error: () => {
        this.summaryCardsError = 'Failed to load summary cards data';
        this.summaryCardsLoading = false;
      },
    });
  }

  onDateRangeChange(range: {
    start: Date | null;
    end: Date | null;
    granularity: string;
  }) {
    this.dashboardFilter = {
      ...this.dashboardFilter,
      startDate: range.start,
      endDate: range.end,
      granularity: range.granularity,
    };
    this.reloadAllWidgets();
  }

  reloadAllWidgets() {
    if (this.chartsComp) {
      this.chartsComp.reloadCharts(this.dashboardFilter);
    }
    if (this.tableComp) {
      this.tableComp.reloadTable(this.dashboardFilter);
    }
    this.reloadSummaryCards();
  }
}
