import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../core/auth.service';

@Component({
  standalone: true, imports: [FormsModule, RouterLink],
  template: `
    <section class="auth-wrap"><div class="auth-panel">
      <div class="eyebrow">BECOME A MEMBER</div><h1>Your movie nights<br><em>start here.</em></h1>
      <form (ngSubmit)="submit()">
        <label>Full name<input name="fullName" [(ngModel)]="fullName" required minlength="2" maxlength="100"></label>
        <label>Email address<input type="email" name="email" [(ngModel)]="email" required email></label>
        <label>Password<input type="password" name="password" [(ngModel)]="password" required minlength="8"></label>
        @if (error) { <p class="error">{{ error }}</p> }
        <button class="btn btn-gold full" [disabled]="busy">{{ busy ? 'Creating account...' : 'Create account' }}</button>
      </form>
      <p class="auth-switch">Already a member? <a routerLink="/login">Sign in</a></p>
    </div></section>
  `
})
export class RegisterComponent {
  fullName = ''; email = ''; password = ''; error = ''; busy = false;
  constructor(private auth: AuthService, private router: Router) {}
  submit(): void {
    this.busy = true; this.error = '';
    this.auth.register(this.fullName, this.email, this.password).subscribe({
      next: () => { this.busy = false; this.router.navigateByUrl('/'); },
      error: err => { this.busy = false; this.error = err?.error?.detail || err?.error?.title || 'Registration failed. Please review your details.'; }
    });
  }
}
