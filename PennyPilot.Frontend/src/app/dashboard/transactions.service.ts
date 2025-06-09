import { Injectable } from '@angular/core';

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
  private incomes: Income[] = [];
  private expenses: Expense[] = [];

  getIncomes(): Income[] {
    return this.incomes;
  }

  getExpenses(): Expense[] {
    return this.expenses;
  }

  addIncomes(incomes: Income[]) {
    this.incomes = [...this.incomes, ...incomes];
  }

  addExpenses(expenses: Expense[]) {
    this.expenses = [...this.expenses, ...expenses];
  }
}
