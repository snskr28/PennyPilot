export interface BarChartDataset{
    label: string;
    data: number[];
}

export interface BarChartsResponse{
    labels: string[];
    datasets: BarChartDataset[];
}