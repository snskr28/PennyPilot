export interface DashboardFilter {
  startDate: Date | null;
  endDate: Date | null;
  granularity: string;
  expenseCategory?: string | null;
  incomeCategory?: string | null;
  userExpense?: string | null;
  incomeSource?: string | null;
}