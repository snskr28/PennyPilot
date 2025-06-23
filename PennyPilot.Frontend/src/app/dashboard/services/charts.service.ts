import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { ApiResponse } from '../../shared/api-response.model';
import { DonutChartsResponse } from '../models/donut-charts-response.model';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ChartsService {
  private apiUrl = environment.apiUrl;
  
  constructor(private http:HttpClient) { }

  getDonutChartsData(): Observable<ApiResponse<DonutChartsResponse>>{
    return this.http.get<ApiResponse<DonutChartsResponse>>(
      `${this.apiUrl}/Charts/DonutCharts`
    );
  }
}
