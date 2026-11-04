import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../core/api.service';
import { AuthService } from '../core/auth.service';
import { ShowSeat } from '../core/models';

@Component({
  standalone: true, imports: [FormsModule, RouterLink],
  template: `
    <section class="content-section">
      <div class="eyebrow">RESERVE YOUR SEATS</div><h1 class="page-title">Choose your <em>spot.</em></h1>
      <p class="muted">Select available seats below. Your selected seats are highlighted in gold.</p>
      @if (error) { <p class="error">{{ error }}</p> }
      <div class="screen-label"><span></span> SCREEN THIS WAY</div>
      <div class="seat-grid">
        @for (seat of seats; track seat.seatId) {
          <button class="seat" [class.selected]="selected.has(seat.seatId)" [disabled]="seat.status.toLowerCase() !== 'available'"
            (click)="toggle(seat.seatId)" [title]="seat.seatLabel + ' · ₹' + seat.price">
            {{ seat.seatLabel || (seat.rowName + seat.seatNumber) }}
          </button>
        }
      </div>
      <div class="seat-legend"><span><i class="dot available"></i> Available</span><span><i class="dot chosen"></i> Selected</span><span><i class="dot unavailable"></i> Unavailable</span></div>
      <div class="booking-panel">
        <div><strong>{{ selected.size }} seats selected</strong><p class="muted">Total: ₹{{ total }}</p></div>
        <form (ngSubmit)="confirm()">
          <input name="phone" [(ngModel)]="phone" placeholder="Mobile number (10–15 digits)" required pattern="[0-9]{10,15}">
          <button class="btn btn-gold" [disabled]="selected.size === 0 || busy">{{ busy ? 'Booking...' : 'Confirm booking' }}</button>
        </form>
      </div>
      @if (success) { <div class="success-panel"><h3>Booking confirmed!</h3><p>{{ success }}</p><a routerLink="/my-bookings">View my bookings →</a></div> }
    </section>
  `
})
export class BookingComponent implements OnInit {
  seats: ShowSeat[] = []; selected = new Set<number>(); phone = '';
  error = ''; success = ''; busy = false; showId = 0;
  constructor(private route: ActivatedRoute, private api: ApiService, public auth: AuthService, private router: Router) {}
  ngOnInit(): void {
    this.showId = Number(this.route.snapshot.paramMap.get('showId'));
    this.api.showSeats(this.showId).subscribe({
      next: data => this.seats = data,
      error: e => this.error = e?.error?.detail || 'Unable to load available seats.'
    });
  }
  toggle(id: number): void {
    if (this.selected.has(id)) this.selected.delete(id); else this.selected.add(id);
  }
  get total(): number { return this.seats.filter(s => this.selected.has(s.seatId)).reduce((sum, s) => sum + s.price, 0); }
  confirm(): void {
    const user = this.auth.user();
    if (!user) { this.router.navigateByUrl('/login'); return; }
    this.busy = true; this.error = '';
    this.api.createBooking({
      customerName: user.fullName, customerEmail: user.email, customerPhone: this.phone,
      showId: this.showId, seatIds: [...this.selected]
    }).subscribe({
      next: booking => {
        this.busy = false; this.selected.clear();
        this.success = `Reference: ${booking.bookingReference}. Total paid/owed: ₹${booking.totalAmount}.`;
      },
      error: e => { this.busy = false; this.error = e?.error?.detail || e?.error?.title || 'Booking failed. Please refresh seats and try again.'; }
    });
  }
}
