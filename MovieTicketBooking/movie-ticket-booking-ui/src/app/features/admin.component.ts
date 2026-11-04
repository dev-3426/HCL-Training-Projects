import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../core/api.service';
import { Booking, Movie, Theatre, Show } from '../core/models';

@Component({
  standalone: true, imports: [FormsModule],
  template: `
    <section class="content-section">
      <div class="eyebrow">CINEBOOK CONTROL ROOM</div><h1 class="page-title">Admin <em>dashboard.</em></h1>
      <div class="admin-tabs">
        @for (tab of tabs; track tab) { <button [class.active]="activeTab===tab" (click)="activeTab=tab">{{ tab }}</button> }
      </div>
      @if (notice) { <p class="success-text">{{ notice }}</p> }
      @if (error) { <p class="error">{{ error }}</p> }
      @if (activeTab === 'Movies') {
        <div class="admin-card"><h3>Add movie</h3>
          <form class="admin-form" (ngSubmit)="addMovie()">
            <input name="title" [(ngModel)]="movieForm.title" placeholder="Title" required>
            <input name="description" [(ngModel)]="movieForm.description" placeholder="Description">
            <input name="duration" type="number" [(ngModel)]="movieForm.durationMinutes" placeholder="Duration (minutes)" required>
            <input name="releaseDate" type="date" [(ngModel)]="movieForm.releaseDate" required>
            <input name="language" [(ngModel)]="movieForm.language" placeholder="Language" required>
            <input name="genre" [(ngModel)]="movieForm.genre" placeholder="Genre" required>
            <button class="btn btn-gold">Add movie</button>
          </form>
        </div>
        <div class="admin-list">@for (m of movies; track m.movieId) { <div><span><b>{{m.title}}</b><small>{{m.genre}} · {{m.language}}</small></span><button class="btn btn-outline small" (click)="deleteMovie(m.movieId)">Delete</button></div> }</div>
      }
      @if (activeTab === 'Theatres') {
        <div class="admin-card"><h3>Add theatre</h3><form class="admin-form" (ngSubmit)="addTheatre()">
          <input name="name" [(ngModel)]="theatreForm.name" placeholder="Theatre name" required>
          <input name="location" [(ngModel)]="theatreForm.location" placeholder="Location" required>
          <button class="btn btn-gold">Add theatre</button>
        </form></div>
        <div class="admin-list">@for (t of theatres; track t.theatreId) { <div><span><b>{{t.name}}</b><small>{{t.location}}</small></span><button class="btn btn-outline small" (click)="deleteTheatre(t.theatreId)">Delete</button></div> }</div>
      }
      @if (activeTab === 'Shows') {
        <div class="admin-card"><h3>Schedule a show</h3><form class="admin-form" (ngSubmit)="addShow()">
          <input name="movieId" type="number" [(ngModel)]="showForm.movieId" placeholder="Movie ID" required>
          <input name="screenId" type="number" [(ngModel)]="showForm.screenId" placeholder="Screen ID" required>
          <label>Start time<input name="start" type="datetime-local" [(ngModel)]="showForm.startTime" required></label>
          <label>End time<input name="end" type="datetime-local" [(ngModel)]="showForm.endTime" required></label>
          <input name="price" type="number" [(ngModel)]="showForm.ticketPrice" placeholder="Ticket price" required>
          <button class="btn btn-gold">Create show</button>
        </form></div>
        <div class="admin-list">@for (s of shows; track s.showId) { <div><span><b>Show #{{s.showId}}</b><small>Movie {{s.movieId}} · Screen {{s.screenId}} · ₹{{s.ticketPrice}}</small></span><button class="btn btn-outline small" (click)="deleteShow(s.showId)">Delete</button></div> }</div>
      }
      @if (activeTab === 'Bookings') {
        <div class="admin-list">@for (b of bookings; track b.bookingId) { <div><span><b>{{b.bookingReference}}</b><small>{{b.customerName}} · Show {{b.showId}} · ₹{{b.totalAmount}} · {{b.status}}</small></span></div> }</div>
      }
      @if (activeTab === 'Screens & seats') {
        <div class="admin-card"><p class="muted">Screen and seat endpoints are connected in the API service. Use the API's screen and seat management endpoints or extend this panel with your preferred bulk-seat layout.</p></div>
      }
    </section>
  `
})
export class AdminComponent implements OnInit {
  tabs = ['Movies','Theatres','Shows','Bookings','Screens & seats'];
  activeTab = 'Movies'; movies: Movie[]=[]; theatres: Theatre[]=[]; shows: Show[]=[]; bookings: Booking[]=[];
  notice=''; error='';
  movieForm: any = { title:'', description:'', durationMinutes:120, releaseDate:'', language:'Tamil', genre:'' };
  theatreForm: any = { name:'', location:'' };
  showForm: any = { movieId:1, screenId:1, startTime:'', endTime:'', ticketPrice:150 };
  constructor(private api: ApiService) {}
  ngOnInit(): void { this.refresh(); }
  refresh(): void {
    this.api.movies().subscribe({next:v=>this.movies=v,error:e=>this.fail(e)});
    this.api.theatres().subscribe({next:v=>this.theatres=v,error:e=>this.fail(e)});
    this.api.shows().subscribe({next:v=>this.shows=v,error:e=>this.fail(e)});
    this.api.adminBookings().subscribe({next:v=>this.bookings=v,error:e=>this.fail(e)});
  }
  addMovie(): void { this.run(this.api.createMovie(this.movieForm), 'Movie added successfully.'); this.movieForm={title:'',description:'',durationMinutes:120,releaseDate:'',language:'Tamil',genre:''}; }
  deleteMovie(id:number): void { if(confirm('Delete this movie?')) this.run(this.api.deleteMovie(id),'Movie deleted.'); }
  addTheatre(): void { this.run(this.api.createTheatre(this.theatreForm),'Theatre added successfully.'); this.theatreForm={name:'',location:''}; }
  deleteTheatre(id:number): void { if(confirm('Delete this theatre?')) this.run(this.api.deleteTheatre(id),'Theatre deleted.'); }
  addShow(): void { this.run(this.api.createShow(this.showForm),'Show created successfully.'); }
  deleteShow(id:number): void { if(confirm('Delete this show?')) this.run(this.api.deleteShow(id),'Show deleted.'); }
  private run(request: any, message: string): void {
    this.notice=''; this.error='';
    request.subscribe({next:()=>{this.notice=message;this.refresh();},error:(e:any)=>this.fail(e)});
  }
  private fail(e:any): void { this.error=e?.error?.detail || e?.error?.title || 'The operation failed. Check your input and API permissions.'; }
}
