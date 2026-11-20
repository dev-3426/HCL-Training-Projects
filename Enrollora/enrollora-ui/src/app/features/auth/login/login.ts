import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../../core/auth/auth.service';
import { LoginRequest } from '../../../core/auth/auth.models';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class Login {
  
  private authService = inject(AuthService);
  private router = inject(Router);

  loginRequest: LoginRequest = {
    email: '',
    password: '',
  };

  isLoading = false;
  errorMessage = '';

  onLogin(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.authService.login(this.loginRequest).subscribe({
      next: (response) => {
        this.isLoading = false;
        if(response.role === 'Administrator') {
          this.router.navigate(['/admin/dashboard']);
        } else if(response.role === 'Student') {
          this.router.navigate(['/student/dashboard']);
        } 
      },

      error: (error) => {
        console.error('Login failed:', error);

        this.errorMessage =
          error.error?.detail || error.error?.message || 'Something went wrong. Please try again.';

        this.isLoading = false;
      },
    });
  }
}
