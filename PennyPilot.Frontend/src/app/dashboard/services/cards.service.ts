import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { DashboardFilter } from '../models/dashboard-filter.model';
import { Observable } from 'rxjs';
import { ApiResponse } from '../../shared/api-response.model';
import { SummaryCardsResponse } from '../models/summary-cards-response.model';

@Injectable({
  providedIn: 'root',
})
export class CardsService {
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getSummaryCardsData(
    dashboardFilter: DashboardFilter
  ): Observable<ApiResponse<SummaryCardsResponse>> {
    return this.http.post<ApiResponse<SummaryCardsResponse>>(
      `${this.apiUrl}/Cards`,
      dashboardFilter
    );
  }
}
