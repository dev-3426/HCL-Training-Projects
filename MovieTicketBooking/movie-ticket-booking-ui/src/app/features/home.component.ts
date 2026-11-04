import { Component, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../core/api.service';
import { Movie } from '../core/models';

@Component({
  standalone: true,
  imports: [RouterLink, FormsModule],
  template: `
    <section class="hero">
      <div class="hero-copy">
        <div class="eyebrow">THE BIG SCREEN EXPERIENCE</div>
        <h1>Stories worth<br><em>showing up for.</em></h1>
        <p>Discover the latest movies, pick your seats, and make tonight unforgettable.</p>
        <a class="btn btn-gold" href="#now-showing">Explore movies <span>→</span></a>
      </div>
      <div class="hero-art"><div class="reel">✦</div><div class="hero-caption">YOUR SEAT<br>IS WAITING</div></div>
    </section>
    <section id="now-showing" class="content-section">
      <div class="section-heading">
        <div><div class="eyebrow">NOW ON THE BIG SCREEN</div><h2>Featured movies</h2></div>
        <input class="search" [(ngModel)]="query" (ngModelChange)="search()" placeholder="Search movies...">
      </div>
      @if (loading) { <p class="muted">Loading movies...</p> }
      @if (error) { <p class="error">{{ error }}</p> }
      <div class="movie-grid">
        @for (movie of movies; track movie.movieId) {
          <a class="movie-card" [routerLink]="['/movies', movie.movieId]">
            <div class="poster" [attr.data-tone]="movie.movieId % 5">
              <span class="poster-symbol">✦</span>
              <span class="poster-title">{{ movie.title }}</span>
              <span class="poster-meta">{{ movie.genre }} · {{ movie.language }}</span>
            </div>
            <div class="movie-info"><h3>{{ movie.title }}</h3><p>{{ movie.durationMinutes }} min <span>·</span> {{ movie.genre }}</p></div>
          </a>
        } @empty { @if (!loading) { <p class="muted">No movies found. Try another search.</p> } }
      </div>
    </section>
  `
})
export class HomeComponent implements OnInit {
  movies: Movie[] = [];
  query = '';
  loading = true;
  error = '';
  constructor(private api: ApiService) {}
  ngOnInit(): void { this.load(); }
  load(): void {
    this.loading = true; this.error = '';
    this.api.movies(1, 50, this.query || undefined).subscribe({
      next: data => { this.movies = data; this.loading = false; },
      error: err => { this.error = this.message(err); this.loading = false; }
    });
  }
  search(): void { this.load(); }
  private message(err: any): string {
    return err?.error?.detail || err?.error?.title || 'Could not connect to the API. Check that your backend is running.';
  }
}
