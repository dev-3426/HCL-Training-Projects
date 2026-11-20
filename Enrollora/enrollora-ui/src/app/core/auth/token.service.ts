
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class TokenService {

  private readonly TOKEN_KEY = 'enrollora_token';
  private readonly ROLE_KEY = 'enrollora_role';

  setToken(token: string): void {
    localStorage.setItem(this.TOKEN_KEY, token);
  }

  getToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  setRole(role: string): void {
    localStorage.setItem(this.ROLE_KEY, role);
  }

  getRole(): string | null {
    return localStorage.getItem(this.ROLE_KEY);
  }

  removeToken(): void {
    localStorage.removeItem(this.TOKEN_KEY);
    localStorage.removeItem(this.ROLE_KEY);
  }

  hasToken(): boolean {
    return this.getToken() !== null;
  }
}
