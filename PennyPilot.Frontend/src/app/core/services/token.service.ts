import { Injectable } from '@angular/core';

const TOKEN_Key = 'auth_token';

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
    return !!this.getToken();
  }
}
