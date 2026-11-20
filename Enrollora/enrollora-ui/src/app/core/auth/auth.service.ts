import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';

import { environment } from '../../../environments/environment';

import { LoginRequest, LoginResponse, RegisterStudentRequest, ApiMessage } from './auth.models';

import { TokenService } from './token.service';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private http = inject(HttpClient);
  private tokenService = inject(TokenService);

  private apiUrl = `${environment.apiUrl}/auth`;

  login(request: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.apiUrl}/login`, request).pipe(
      tap((response) => {
        this.tokenService.setToken(response.token);
        this.tokenService.setRole(response.role);
      }),
    );
  }

  register(request: RegisterStudentRequest): Observable<ApiMessage> {
    return this.http.post<ApiMessage>(`${this.apiUrl}/register`, request);
  }

  logout(): void {
    this.tokenService.removeToken();
  }
}
