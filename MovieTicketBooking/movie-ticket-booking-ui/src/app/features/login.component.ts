import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../core/auth.service';

@Component({
  standalone: true, imports: [FormsModule, RouterLink],
  template: `
    <section class="auth-wrap"><div class="auth-panel">
      <div class="eyebrow">WELCOME BACK</div><h1>Good to see<br><em>you again.</em></h1>
      <p class="muted">Sign in to continue your cinema experience.</p>
      <form (ngSubmit)="submit()">
        <label>Email address<input type="email" name="email" [(ngModel)]="email" required email></label>
        <label>Password<input type="password" name="password" [(ngModel)]="password" required></label>
        @if (error) { <p class="error">{{ error }}</p> }
        <button class="btn btn-gold full" [disabled]="busy">{{ busy ? 'Signing in...' : 'Sign in' }}</button>
      </form>
      <p class="auth-switch">New to CineBook? <a routerLink="/register">Create an account</a></p>
    </div></section>
  `
})
export class LoginComponent {
  email = ''; password = ''; error = ''; busy = false;
  constructor(private auth: AuthService, private router: Router) {}
  submit(): void {
    this.busy = true; this.error = '';
    this.auth.login(this.email, this.password).subscribe({
      next: result => { this.busy = false; this.router.navigateByUrl(result.role.toLowerCase() === 'admin' ? '/admin' : '/'); },
      error: err => { this.busy = false; this.error = err?.error?.detail || err?.error?.title || 'Login failed. Check your email and password.'; }
    });
  }
}
