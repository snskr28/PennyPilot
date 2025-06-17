import { Injectable } from '@angular/core';
import {jwtDecode} from 'jwt-decode';

const TOKEN_Key = 'auth_token';

interface JWTPayload{
  exp: number;
  [key: string]: any;
}

@Injectable({ providedIn: 'root' })
export class TokenService {
  saveToken(token: string): void {
    localStorage.setItem(TOKEN_Key, token);
  }

  getToken(): string | null {
    return localStorage.getItem(TOKEN_Key);
  }

  removeToken(): void {
    localStorage.removeItem(TOKEN_Key);
  }

  isLoggedIn(): boolean {
    const token = this.getToken();
    if (!token) return false;

    try {
      const payload = jwtDecode<JWTPayload>(token);
      const now = Math.floor(Date.now() / 1000);
      return payload.exp > now; 
    } catch (e) {
      return false;
    }
  }
}
