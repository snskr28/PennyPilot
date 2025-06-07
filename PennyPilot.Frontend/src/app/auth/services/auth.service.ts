import { HttpClient } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { TokenService } from "../../core/services/token.service";
import { AuthResponse, LoginRequest, SignupRequest } from "../models/auth.model";
import { Observable, tap } from "rxjs";
import { Router } from "@angular/router";

@Injectable({providedIn: 'root'})
export class AuthService{
    private http = inject(HttpClient);
    private tokenService = inject(TokenService)
    private router = inject(Router);
    private apiUrl = 'https://localhost:7098/api/auth'

    login(payload: LoginRequest): Observable<AuthResponse>{
        return this.http.post<AuthResponse>(`${this.apiUrl}/login`, payload).pipe(
            tap((res) => this.tokenService.saveToken(res.token))
        );
    }

    signup(payload: SignupRequest): Observable<any>{
        return this.http.post(`${this.apiUrl}/register`, payload);
    }

    logout(): void{
        this.tokenService.removeToken();
        this.router.navigate(['/login']);
    }

    
    forgotPassword(identifier: string): Observable<any> {
        return this.http.post(`${this.apiUrl}/forgot-password`, { identifier });
    }

    resetPassword(token: string, newPassword: string): Observable<any> {
        return this.http.post(`${this.apiUrl}/reset-password`, { token, newPassword });
    }
}