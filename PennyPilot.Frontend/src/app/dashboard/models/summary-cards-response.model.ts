export interface SummaryCardsResponse {
  totalIncome?: {name: string; value: number};
  totalExpenses?: {name: string; value: number};
  netSavings?: {name: string; value: number};
}
