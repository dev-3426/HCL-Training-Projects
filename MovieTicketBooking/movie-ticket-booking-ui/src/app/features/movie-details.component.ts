import { Component, OnInit } from '@angular/core';
import { DatePipe } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ApiService } from '../core/api.service';
import { Movie, Show, Theatre, Screen } from '../core/models';

@Component({
  standalone: true, imports: [RouterLink, DatePipe],
  template: `
    <section class="content-section detail-section">
      @if (movie) {
        <div class="detail-hero">
          <div class="detail-poster"><span>✦</span><strong>{{ movie.title }}</strong><small>{{ movie.genre }} · {{ movie.language }}</small></div>
          <div class="detail-copy"><div class="eyebrow">MOVIE DETAILS</div><h1>{{ movie.title }}</h1>
            <p>{{ movie.description || 'Get ready for an unforgettable big-screen experience.' }}</p>
            <div class="meta-pills"><span>{{ movie.durationMinutes }} minutes</span><span>{{ movie.language }}</span><span>{{ movie.genre }}</span></div>
            <p class="muted">Release date: {{ movie.releaseDate | date:'mediumDate' }}</p>
          </div>
        </div>
        <div class="section-heading"><div><div class="eyebrow">CHOOSE YOUR SHOW</div><h2>Available shows</h2></div></div>
        @if (error) { <p class="error">{{ error }}</p> }
        <div class="show-list">
          @for (show of shows; track show.showId) {
            <div class="show-card"><div><strong>{{ show.startTime | date:'EEE, d MMM · h:mm a' }}</strong><p class="muted">Show #{{ show.showId }} · Screen #{{ show.screenId }}</p></div>
              <div class="show-action"><strong>₹{{ show.ticketPrice }}</strong><a class="btn btn-gold small" [routerLink]="['/book', show.showId]">Select seats</a></div>
            </div>
          } @empty { @if (!error) { <p class="muted">No shows are currently listed for this movie.</p> } }
        </div>
      } @else if (error) { <p class="error">{{ error }}</p> } @else { <p class="muted">Loading movie...</p> }
    </section>
  `
})
export class MovieDetailsComponent implements OnInit {
  movie?: Movie; shows: Show[] = []; error = '';
  constructor(private route: ActivatedRoute, private api: ApiService) { }
  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.api.movie(id).subscribe({ next: m => this.movie = m, error: e => this.error = e?.error?.detail || 'Unable to load movie.' });
    this.api.shows({ movieId: id }).subscribe({ next: s => this.shows = s, error: e => this.error = e?.error?.detail || 'Unable to load shows.' });
  }
}
