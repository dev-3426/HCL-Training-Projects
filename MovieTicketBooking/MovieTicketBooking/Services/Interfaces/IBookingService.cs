using MovieTicketBooking.DTOs.Bookings;

namespace MovieTicketBooking.Services.Interfaces;

public interface IBookingService
{
    Task<BookingResponse> CreateAsync(
        CreateBookingRequest request);

    Task<BookingResponse?> GetByIdAsync(int bookingId);

    Task<List<BookingResponse>> GetAllAsync(
        int? showId,
        string? status);

    Task<List<BookingResponse>> GetByShowIdAsync(int showId);

    Task<BookingResponse?> CancelAsync(int bookingId);
}