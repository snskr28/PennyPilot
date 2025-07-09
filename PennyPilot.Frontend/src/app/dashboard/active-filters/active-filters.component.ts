import { Component, EventEmitter, Input, Output } from '@angular/core';
import { MATERIAL_IMPORTS } from '../../shared/material';
import { DashboardFilter } from '../models/dashboard-filter.model';
import { filter } from 'rxjs';
import { CommonModule } from '@angular/common';
import { PascalCasePipe } from '../../shared/Pipes/pascal-case.pipe';

@Component({
  selector: 'app-active-filters',
  imports: [CommonModule, PascalCasePipe,...MATERIAL_IMPORTS],
  templateUrl: './active-filters.component.html',
  styleUrl: './active-filters.component.scss',
})
export class ActiveFiltersComponent {
  @Input() dashboardFilter!: DashboardFilter;
  @Output() filterCleared = new EventEmitter<string>();

  get activeFilters() {
    const f = this.dashboardFilter;
    const filters: { key: string; label: string; value: string }[] = [];
    if (f.startDate || f.endDate) {
      filters.push({
        key: 'date',
        label: 'Date',
        value: `${
          f.startDate ? (f.startDate as Date).toLocaleDateString() : ''
        }${f.startDate && f.endDate ? ' - ' : ''}${
          f.endDate ? (f.endDate as Date).toLocaleDateString() : ''
        }`,
      });
    }
    if (f.granularity) {
      filters.push({
        key: 'granularity',
        label: 'Granularity',
        value: f.granularity,
      });
    }
    if (f.expenseCategory) {
      filters.push({
        key: 'expenseCategory',
        label: 'Expense Category',
        value: f.expenseCategory,
      });
    }
    if (f.incomeCategory) {
      filters.push({
        key: 'incomeCategory',
        label: 'Income Category',
        value: f.incomeCategory,
      });
    }
    if (f.userExpense) {
      filters.push({
        key: 'userExpense',
        label: 'User Expense',
        value: f.userExpense,
      });
    }
    if (f.incomeSource) {
      filters.push({
        key: 'incomeSource',
        label: 'Income Source',
        value: f.incomeSource,
      });
    }
    return filters;
  }

  clearFilter(key: string) {
    this.filterCleared.emit(key);
  }
}
