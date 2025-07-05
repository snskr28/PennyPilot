import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { ApiResponse } from '../../shared/api-response.model';
import { DonutChartsResponse } from '../models/donut-charts-response.model';
import { Observable } from 'rxjs';
import { DashboardFilter } from '../models/dashboard-filter.model';
import { BarChartsResponse } from '../models/bar-charts-response.model';

@Injectable({
  providedIn: 'root',
})
export class ChartsService {
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getDonutChartsData(
    dashboardFilter: DashboardFilter
  ): Observable<ApiResponse<DonutChartsResponse>> {
    return this.http.post<ApiResponse<DonutChartsResponse>>(
      `${this.apiUrl}/Charts/DonutCharts`,
      dashboardFilter
    );
  }

  getIncomeExpenseBarChartData(
    dashboardFilter: DashboardFilter
  ): Observable<ApiResponse<BarChartsResponse>>{
    return this.http.post<ApiResponse<BarChartsResponse>>(
      `${this.apiUrl}/Charts/IncomeExpenseBarChart`,
      dashboardFilter
    );
  }
}
