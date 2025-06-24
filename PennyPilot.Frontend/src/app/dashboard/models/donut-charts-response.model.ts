export interface DonutChartsResponse {
    expenseCategories?: {[category: string]: number} ;
    userExpenses?: {[date: string]: number} ;
    incomeCategories?: {[category: string]: number} ;
    incomeSources?: {[source: string]: number} ;
}
