import { Component } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { AuthService } from './core/auth.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  template: `
    <header class="topbar">
      <a class="brand" routerLink="/"><span class="brand-mark">C</span> CINEBOOK</a>
      <nav>
        <a routerLink="/" routerLinkActive="active" [routerLinkActiveOptions]="{exact:true}">Movies</a>
        @if (auth.isLoggedIn() && !auth.isAdmin()) {
          <a routerLink="/my-bookings" routerLinkActive="active">My bookings</a>
        }
        @if (auth.isAdmin()) { <a routerLink="/admin" routerLinkActive="active">Admin</a> }
      </nav>
      <div class="account">
        @if (auth.isLoggedIn()) {
          <span class="welcome">Hi, {{ auth.user()?.fullName }}</span>
          <button class="btn btn-outline small" (click)="logout()">Sign out</button>
        } @else {
          <a class="login-link" routerLink="/login">Sign in</a>
          <a class="btn btn-gold small" routerLink="/register">Join now</a>
        }
      </div>
    </header>
    <main><router-outlet /></main>
    <footer>© 2026 CineBook · Your next movie night starts here.</footer>
  `
})
export class AppComponent {
  constructor(public auth: AuthService) {}
  logout(): void { this.auth.logout(); location.href = '/'; }
}
