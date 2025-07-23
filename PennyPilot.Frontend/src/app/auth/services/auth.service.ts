import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { TokenService } from '../../core/services/token.service';
import {
  AuthResponse,
  LoginRequest,
  SignupRequest,
} from '../models/auth.model';
import { Observable, tap } from 'rxjs';
import { Router } from '@angular/router';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private http = inject(HttpClient);
  private tokenService = inject(TokenService);
  private router = inject(Router);
  private apiUrl = `${environment.apiUrl}/auth`;

  login(payload: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/login`, payload).pipe(
      tap((res) => {
        this.tokenService.saveToken(res.data.token);
        localStorage.setItem('username', res.data.username);
        localStorage.setItem('email', res.data.email);
      })
    );
  }

  signup(payload: SignupRequest): Observable<any> {
    return this.http.post(`${this.apiUrl}/register`, payload);
  }

  logout(): void {
    this.tokenService.removeToken();
    localStorage.removeItem('username');
    localStorage.removeItem('email');
    this.router.navigate(['/login']);
  }

  forgotPassword(identifier: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/forgot-password`, { identifier });
  }

  resetPassword(token: string, newPassword: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/reset-password`, {
      token,
      newPassword,
    });
  }
}
