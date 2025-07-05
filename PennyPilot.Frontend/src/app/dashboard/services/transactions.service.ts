import { Inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { TableRequest } from '../models/table-request.model';
import { Observable } from 'rxjs';
import { TableResponse } from '../models/table-response.model';
import { HttpClient } from '@angular/common/http';
import { ApiResponse } from '../../shared/api-response.model';

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

  constructor(private http: HttpClient) {}

  getIncomeTable(
    request: TableRequest
  ): Observable<ApiResponse<TableResponse<Income>>> {
    return this.http.post<ApiResponse<TableResponse<Income>>>(
      `${this.apiUrl}/Income/IncomeTable`,
      request
    );
  }

  getExpenseTable(
    request: TableRequest
  ): Observable<ApiResponse<TableResponse<Expense>>> {
    return this.http.post<ApiResponse<TableResponse<Expense>>>(
      `${this.apiUrl}/Expense/ExpenseTable`,
      request
    );
  }

  addIncomes(incomes: Income[]): Observable<ApiResponse<string[]>> {
    return this.http.post<ApiResponse<string[]>>(
      `${this.apiUrl}/Income`,
      incomes
    );
  }

  addExpenses(expenses: Expense[]): Observable<ApiResponse<string[]>> {
    return this.http.post<ApiResponse<string[]>>(
      `${this.apiUrl}/Expense`,
      expenses
    );
  }
}
