import { Injectable, computed, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { tap } from 'rxjs';
import { AuthResponse } from './models';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly storageKey = 'cinebook-auth';
  private readonly current = signal<AuthResponse | null>(this.readStored());
  readonly user = this.current.asReadonly();
  readonly isLoggedIn = computed(() => !!this.current()?.token &&
    (!this.current()?.expiresAt || new Date(this.current()!.expiresAt) > new Date()));
  readonly isAdmin = computed(() => this.current()?.role?.toLowerCase() === 'admin');

  constructor(private http: HttpClient) {}

  private readStored(): AuthResponse | null {
    try {
      const raw = localStorage.getItem(this.storageKey);
      return raw ? JSON.parse(raw) as AuthResponse : null;
    } catch { return null; }
  }

  login(email: string, password: string) {
    return this.http.post<AuthResponse>('/api/auth/login', { email, password }).pipe(
      tap(result => this.save(result))
    );
  }

  register(fullName: string, email: string, password: string) {
    return this.http.post<AuthResponse>('/api/auth/register', { fullName, email, password }).pipe(
      tap(result => this.save(result))
    );
  }

  private save(value: AuthResponse): void {
    localStorage.setItem(this.storageKey, JSON.stringify(value));
    this.current.set(value);
  }

  logout(): void {
    localStorage.removeItem(this.storageKey);
    this.current.set(null);
  }

  token(): string | null { return this.isLoggedIn() ? this.current()?.token ?? null : null; }
}
