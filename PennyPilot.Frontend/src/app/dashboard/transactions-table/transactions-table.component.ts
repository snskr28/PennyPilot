import { Component, Input, SimpleChanges, ViewChild, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatDialog } from '@angular/material/dialog';
import { AddTransactionDialogComponent } from '../add-transaction-dialog/add-transaction-dialog.component';
import {
  TransactionsService,
  Income,
  Expense,
} from '../services/transactions.service';
import { MATERIAL_IMPORTS } from '../../shared/material';
import { MatTableDataSource } from '@angular/material/table';
import {
  catchError,
  finalize,
  map,
  merge,
  Observable,
  startWith,
  switchMap,
} from 'rxjs';
import { DashboardFilter } from '../models/dashboard-filter.model';

@Component({
  selector: 'app-transactions-table',
  standalone: true,
  imports: [CommonModule, ...MATERIAL_IMPORTS],
  templateUrl: './transactions-table.component.html',
  styleUrls: ['./transactions-table.component.scss'],
})
export class TransactionsTableComponent {
  @Input() dashboardFilter!: DashboardFilter;

  private dialog = inject(MatDialog);
  private transactionsService = inject(TransactionsService);

  activeTab: 'income' | 'expense' = 'income';
  loading = false;
  error: string | null = null;
  totalItems = 0;

  incomeDisplayedColumns = [
    'sn',
    'title',
    'category',
    'description',
    'source',
    'amount',
    'date',
  ];
  expenseDisplayedColumns = [
    'sn',
    'title',
    'category',
    'description',
    'amount',
    'paidBy',
    'paymentMode',
    'date',
  ];

  incomeDataSource = new MatTableDataSource<Income>([]);
  expenseDataSource = new MatTableDataSource<Expense>([]);

  @ViewChild('incomePaginator') incomePaginator!: MatPaginator;
  @ViewChild('incomeSort') incomeSort!: MatSort;
  @ViewChild('expensePaginator') expensePaginator!: MatPaginator;
  @ViewChild('expenseSort') expenseSort!: MatSort;

  ngAfterViewInit() {
    this.setupIncomeTable();
    this.setupExpenseTable();
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['filter'] && !changes['filter'].firstChange) {
      this.reloadTable(this.dashboardFilter);
    }
  }

  reloadTable(filter: DashboardFilter) {
    // Reset paginators to first page
    if (this.activeTab === 'income' && this.incomePaginator) {
      this.incomePaginator.pageIndex = 0;
      this.setupIncomeTable();
    } else if (this.activeTab === 'expense' && this.expensePaginator) {
      this.expensePaginator.pageIndex = 0;
      this.setupExpenseTable();
    }
  }

  private setupIncomeTable() {
    merge(this.incomeSort.sortChange, this.incomePaginator.page)
      .pipe(
        startWith({}),
        switchMap(() => {
          this.loading = true;
          this.error = null;
          return this.transactionsService.getIncomeTable({
            pageNumber: this.incomePaginator.pageIndex + 1,
            pageSize: this.incomePaginator.pageSize,
            sortBy: this.incomeSort.active || 'date',
            sortOrder: this.incomeSort.direction || 'desc',
            dashboardFilter: this.dashboardFilter,
          });
        }),
        map((response: any) => {
          if (response.success) {
            this.totalItems = response.data.totalCount;
            this.loading = false;
            return response.data.items;
          }
          throw new Error(response.message);
        }),
        catchError((error) => {
             
          return [];
        })
      )
      .subscribe((data) => {
        this.incomeDataSource.data = data;
      });
  }

  private setupExpenseTable() {
    merge(this.expenseSort.sortChange, this.expensePaginator.page)
      .pipe(
        startWith({}),
        switchMap(() => {
          this.loading = true;
          this.error = null;
          return this.transactionsService.getExpenseTable({
            pageNumber: this.expensePaginator.pageIndex + 1,
            pageSize: this.expensePaginator.pageSize,
            sortBy: this.expenseSort.active || 'date',
            sortOrder: this.expenseSort.direction || 'desc',
            dashboardFilter: this.dashboardFilter,
          });
        }),
        map((response: any) => {
          if (response.success) {
            this.totalItems = response.data.totalCount;
            this.loading = false;
            return response.data.items;
          }
          throw new Error(response.message);
        }),
        catchError((error) => {
          this.error =
            error.message || 'An error occurred while fetching expense data.';
          console.error('Error fetching expenses:', error);
          this.loading = false;
          return [];
        })
      )
      .subscribe((data) => {
        this.expenseDataSource.data = data;
      });
  }

  onTabChange(event: any) {
    this.activeTab = event.index === 0 ? 'income' : 'expense';
    this.error = null;

    if (this.activeTab === 'income') {
      this.incomePaginator.pageIndex = 0;
      this.incomeSort.active = 'date';
      this.incomeSort.direction = 'desc';
      this.incomeSort.sortChange.emit(); // Triggers reload
    } else {
      this.expensePaginator.pageIndex = 0;
      this.expenseSort.active = 'date';
      this.expenseSort.direction = 'desc';
      this.expenseSort.sortChange.emit(); // Triggers reload
    }
  }
  openAddTransactionDialog() {
    const dialogRef = this.dialog.open(AddTransactionDialogComponent, {
      width: '600px',
      data: { type: this.activeTab },
    });

    dialogRef.afterClosed().subscribe((refreshNeeded) => {
      if (refreshNeeded && this.activeTab === 'expense') {
        this.setupExpenseTable();
      } else if (refreshNeeded && this.activeTab === 'income') {
        this.setupIncomeTable();
      }
    });
  }
}
