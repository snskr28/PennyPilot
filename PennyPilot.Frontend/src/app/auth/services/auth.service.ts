import { HttpClient } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { TokenService } from "../../core/services/token.service";
import { AuthResponse, LoginRequest, SignupRequest } from "../models/auth.model";
import { Observable, tap } from "rxjs";

@Injectable({providedIn: 'root'})
export class AuthService{
    private http = inject(HttpClient);
    private tokenService = inject(TokenService)
    private apiUrl = 'https://localhost:7098/api/User'

    login(payload: LoginRequest): Observable<AuthResponse>{
        return this.http.post<AuthResponse>(`${this.apiUrl}/login`, payload).pipe(
            tap((res) => this.tokenService.saveToken(res.token))
        );
    }

    signup(payload: SignupRequest): Observable<any>{
        return this.http.post(`${this.apiUrl}/register`, payload);
    }

    logout(): void{
        this.tokenService.removeToken()
    }
}