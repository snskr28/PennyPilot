import { Component, ViewChild, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTabsModule } from '@angular/material/tabs';
import { MatTableModule } from '@angular/material/table';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { MatSort, MatSortModule } from '@angular/material/sort';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { AddTransactionDialogComponent } from '../add-transaction-dialog/add-transaction-dialog.component';
import { TransactionsService, Income, Expense } from '../transactions.service';
import { MATERIAL_IMPORTS } from '../../shared/material';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatCardModule } from '@angular/material/card';
import { MatTableDataSource } from '@angular/material/table';

@Component({
  selector: 'app-transactions-table',
  standalone: true,
  imports: [
    CommonModule,
    MatTabsModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatButtonModule,
    MatDialogModule,
    MatIconModule,
    MatTooltipModule,
    MatCardModule,
    ...MATERIAL_IMPORTS
  ],
  templateUrl: './transactions-table.component.html',
  styleUrls: ['./transactions-table.component.scss']
})
export class TransactionsTableComponent {
  private dialog = inject(MatDialog);
  private transactionsService = inject(TransactionsService);

  activeTab: 'income' | 'expense' = 'income';

  incomeDisplayedColumns = ['sn', 'category', 'source', 'description', 'amount', 'date'];
  expenseDisplayedColumns = ['sn', 'title', 'category', 'description', 'amount', 'paymentMode', 'paidBy', 'date'];

  incomeDataSource = new MatTableDataSource<Income>([]);
  expenseDataSource = new MatTableDataSource<Expense>([]);

  @ViewChild('incomePaginator') incomePaginator!: MatPaginator;
  @ViewChild('incomeSort') incomeSort!: MatSort;
  @ViewChild('expensePaginator') expensePaginator!: MatPaginator;
  @ViewChild('expenseSort') expenseSort!: MatSort;

  ngAfterViewInit() {
    this.incomeDataSource.paginator = this.incomePaginator;
    this.incomeDataSource.sort = this.incomeSort;
    this.expenseDataSource.paginator = this.expensePaginator;
    this.expenseDataSource.sort = this.expenseSort;
    this.loadData();
  }

  loadData() {
    this.incomeDataSource.data = this.transactionsService.getIncomes();
    this.expenseDataSource.data = this.transactionsService.getExpenses();
  }

  openAddDialog() {
    const dialogRef = this.dialog.open(AddTransactionDialogComponent, {
      width: '600px',
      data: { type: this.activeTab }
    });
    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        if (this.activeTab === 'income') {
          this.transactionsService.addIncomes(result);
        } else {
          this.transactionsService.addExpenses(result);
        }
        this.loadData();
      }
    });
  }

  onTabChange(event: any) {
    this.activeTab = event.index === 0 ? 'income' : 'expense';
  }
}
