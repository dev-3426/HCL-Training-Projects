import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Booking, Movie, Screen, Seat, Show, ShowSeat, Theatre } from './models';

@Injectable({ providedIn: 'root' })
export class ApiService {
  // Change this URL if your ASP.NET Core API uses a different port.
  private readonly baseUrl = 'https://localhost:7120/api';
  constructor(private http: HttpClient) {}

  private url(path: string): string { return `${this.baseUrl}/${path}`; }

  movies(pageNumber = 1, pageSize = 50, title?: string): Observable<Movie[]> {
    let params = new HttpParams().set('pageNumber', pageNumber).set('pageSize', pageSize);
    if (title) params = params.set('title', title);
    return this.http.get<Movie[]>(this.url('movies'), { params });
  }
  movie(id: number): Observable<Movie> { return this.http.get<Movie>(this.url(`movies/${id}`)); }
  searchMovies(title: string): Observable<Movie[]> {
    return this.http.get<Movie[]>(this.url('movies/search'), { params: { title } });
  }
  theatres(): Observable<Theatre[]> { return this.http.get<Theatre[]>(this.url('theatres')); }
  screens(theatreId?: number): Observable<Screen[]> {
    let params = new HttpParams();
    if (theatreId) params = params.set('theatreId', theatreId);
    return this.http.get<Screen[]>(this.url('screens'), { params });
  }
  shows(filters?: { date?: string; movieId?: number; theatreId?: number }): Observable<Show[]> {
    let params = new HttpParams();
    if (filters?.date) params = params.set('date', filters.date);
    if (filters?.movieId) params = params.set('movieId', filters.movieId);
    if (filters?.theatreId) params = params.set('theatreId', filters.theatreId);
    return this.http.get<Show[]>(this.url('shows'), { params });
  }
  showSeats(showId: number): Observable<ShowSeat[]> {
    return this.http.get<ShowSeat[]>(this.url(`shows/${showId}/seats/available`));
  }
  createBooking(data: {
    customerName: string; customerEmail: string; customerPhone: string;
    showId: number; seatIds: number[];
  }): Observable<Booking> {
    return this.http.post<Booking>(this.url('bookings'), data);
  }
  myBookings(): Observable<Booking[]> {
    return this.http.get<Booking[]>(this.url('bookings/my-bookings'));
  }
  cancelMyBooking(id: number): Observable<unknown> {
    return this.http.put(this.url(`bookings/${id}/cancel-my-booking`), {});
  }
  adminBookings(): Observable<Booking[]> { return this.http.get<Booking[]>(this.url('bookings')); }

  createMovie(data: Partial<Movie>) { return this.http.post(this.url('movies'), data); }
  updateMovie(id: number, data: Partial<Movie>) { return this.http.put(this.url(`movies/${id}`), data); }
  deleteMovie(id: number) { return this.http.delete(this.url(`movies/${id}`)); }
  createTheatre(data: Partial<Theatre>) { return this.http.post(this.url('theatres'), data); }
  updateTheatre(id: number, data: Partial<Theatre>) { return this.http.put(this.url(`theatres/${id}`), data); }
  deleteTheatre(id: number) { return this.http.delete(this.url(`theatres/${id}`)); }
  createScreen(data: Partial<Screen>) { return this.http.post(this.url('screens'), data); }
  updateScreen(id: number, data: Partial<Screen>) { return this.http.put(this.url(`screens/${id}`), data); }
  deleteScreen(id: number) { return this.http.delete(this.url(`screens/${id}`)); }
  createSeat(data: Partial<Seat>) { return this.http.post(this.url('seats'), data); }
  bulkCreateSeats(data: { screenId: number; seats: { rowName: string; seatNumber: number; seatType: string }[] }) {
    return this.http.post(this.url('seats/bulk'), data);
  }
  updateSeat(id: number, data: Partial<Seat>) { return this.http.put(this.url(`seats/${id}`), data); }
  deleteSeat(id: number) { return this.http.delete(this.url(`seats/${id}`)); }
  createShow(data: Partial<Show>) { return this.http.post(this.url('shows'), data); }
  updateShow(id: number, data: Partial<Show>) { return this.http.put(this.url(`shows/${id}`), data); }
  deleteShow(id: number) { return this.http.delete(this.url(`shows/${id}`)); }
  cancelBooking(id: number) { return this.http.put(this.url(`bookings/${id}/cancel`), {}); }
}
