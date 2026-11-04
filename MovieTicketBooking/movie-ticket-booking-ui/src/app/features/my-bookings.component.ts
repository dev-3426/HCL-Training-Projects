import { Component, OnInit } from '@angular/core';
import { DatePipe, CurrencyPipe } from '@angular/common';
import { ApiService } from '../core/api.service';
import { Booking } from '../core/models';

@Component({
  standalone: true, imports: [DatePipe, CurrencyPipe],
  template: `
    <section class="content-section"><div class="eyebrow">YOUR CINEBOOK ACCOUNT</div><h1 class="page-title">My <em>bookings.</em></h1>
      @if (error) { <p class="error">{{ error }}</p> }
      @for (booking of bookings; track booking.bookingId) {
        <article class="booking-history-card">
          <div class="booking-head"><div><span class="eyebrow">BOOKING REFERENCE</span><h3>{{ booking.bookingReference }}</h3></div><span class="status">{{ booking.status }}</span></div>
          <p class="muted">Show #{{ booking.showId }} · {{ booking.bookingDate | date:'medium' }}</p>
          <div class="chips">@for (seat of booking.seats; track seat.seatId) { <span>{{ seat.seatLabel }}</span> }</div>
          <div class="booking-foot"><strong>{{ booking.totalAmount | currency:'INR':'symbol':'1.0-2' }}</strong>
            @if (booking.status.toLowerCase() !== 'cancelled') { <button class="btn btn-outline small" (click)="cancel(booking)">Cancel booking</button> }
          </div>
        </article>
      } @empty { @if (!error) { <div class="empty-state"><div>✦</div><h3>No bookings yet</h3><p class="muted">Your next movie night is just a few clicks away.</p><a href="/">Explore movies</a></div> } }
    </section>
  `
})
export class MyBookingsComponent implements OnInit {
  bookings: Booking[] = []; error = '';
  constructor(private api: ApiService) {}
  ngOnInit(): void { this.load(); }
  load(): void { this.api.myBookings().subscribe({ next: b => this.bookings = b, error: e => this.error = e?.error?.detail || 'Unable to load bookings.' }); }
  cancel(booking: Booking): void {
    if (!confirm(`Cancel booking ${booking.bookingReference}?`)) return;
    this.api.cancelMyBooking(booking.bookingId).subscribe({ next: () => this.load(), error: e => this.error = e?.error?.detail || 'Could not cancel booking.' });
  }
}
