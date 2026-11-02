
using MovieTicketBooking.DTOs.Bookings;

namespace MovieTicketBooking.Services.Interfaces;

public interface IBookingService
{
    // Customer: Create booking
    Task<BookingResponse> CreateAsync(
        CreateBookingRequest request,
        int userId);

    // Admin: Get booking by ID
    Task<BookingResponse?> GetByIdAsync(int bookingId);

    // Customer: Get own booking by ID
    Task<BookingResponse?> GetByIdForUserAsync(
        int bookingId,
        int userId);

    // Admin: Get all bookings
    Task<List<BookingResponse>> GetAllAsync(
        int? showId,
        string? status);

    // Admin: Get bookings by show
    Task<List<BookingResponse>> GetByShowIdAsync(int showId);

    // Customer: Get own bookings
    Task<List<BookingResponse>> GetMyBookingsAsync(int userId);

    // Admin: Cancel any booking
    Task<BookingResponse?> CancelAsync(int bookingId);

    // Customer: Cancel own booking
    Task<BookingResponse?> CancelForUserAsync(
        int bookingId,
        int userId);
}
