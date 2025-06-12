import { Inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { TableRequest } from '../models/table-request.model';
import { Observable } from 'rxjs';
import { TableResponse } from '../models/table-response.model';
import { HttpClient } from '@angular/common/http';

export interface Income {
  category: string;
  source: string;
  description: string;
  amount: number;
  date: string;
}

export interface Expense {
  title: string;
  category: string;
  description: string;
  amount: number;
  paymentMode: string;
  paidBy: string;
  date: string;
}

@Injectable({ providedIn: 'root' })
export class TransactionsService {
  private apiUrl = environment.apiUrl;

  private incomes: Income[] = [];
  private expenses: Expense[] = [];

  constructor(private http: HttpClient) {}

  getIncomeTable(request: TableRequest): Observable<TableResponse<Income>> {
    return this.http.post<TableResponse<Income>>(`${this.apiUrl}/Income/IncomeTable`, request);
  }

  getExpenseTable(request: TableRequest): Observable<TableResponse<Expense>> {
    return this.http.post<TableResponse<Expense>>(`${this.apiUrl}/Expense/ExpenseTable`, request);
  }

  addIncomes(incomes: Income[]) {
    this.incomes = [...this.incomes, ...incomes];
  }

  addExpenses(expenses: Expense[]) {
    this.expenses = [...this.expenses, ...expenses];
  }
}
