export interface AuthResponse {
  userId: number;
  fullName: string;
  email: string;
  role: string;
  token: string;
  expiresAt: string;
}
export interface Movie {
  movieId: number;
  title: string;
  description?: string | null;
  durationMinutes: number;
  releaseDate: string;
  language: string;
  genre: string;
}
export interface Theatre { theatreId: number; name: string; location: string; }
export interface Screen { screenId: number; theatreId: number; screenName: string; }
export interface Seat {
  seatId: number; screenId: number; rowName: string; seatNumber: number; seatType: string;
}
export interface Show {
  showId: number; movieId: number; screenId: number;
  startTime: string; endTime: string; ticketPrice: number;
}
export interface ShowSeat {
  seatId: number; rowName: string; seatNumber: number;
  seatLabel: string; seatType: string; price: number; status: string;
}
export interface BookingSeat {
  seatId: number; rowName: string; seatNumber: number;
  seatLabel: string; price: number;
}
export interface Booking {
  bookingId: number; bookingReference: string; showId: number;
  customerName: string; customerEmail: string; customerPhone: string;
  bookingDate: string; totalAmount: number; status: string; seats: BookingSeat[];
}
