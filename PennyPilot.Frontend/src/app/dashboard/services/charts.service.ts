import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { ApiResponse } from '../../shared/api-response.model';
import { PieChartsResponse } from '../models/pie-charts-response.model';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ChartsService {
  private apiUrl = environment.apiUrl;
  
  constructor(private http:HttpClient) { }

  getPieChartsData(): Observable<ApiResponse<PieChartsResponse>>{
    return this.http.get<ApiResponse<PieChartsResponse>>(
      `${this.apiUrl}/Charts/PieCharts`
    );
  }
}
