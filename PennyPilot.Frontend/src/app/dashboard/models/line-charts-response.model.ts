export interface LineChartDataset{
    label: string;
    data: number[];
}

export interface LineChartsResponse{
    labels: string[];
    datasets: LineChartDataset[];
}