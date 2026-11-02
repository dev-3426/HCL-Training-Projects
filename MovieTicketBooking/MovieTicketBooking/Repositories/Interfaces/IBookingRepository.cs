
using MovieTicketBooking.Models;

namespace MovieTicketBooking.Repositories.Interfaces;

public interface IBookingRepository
{
    Task<Show?> GetShowByIdAsync(int showId);

    Task<List<ShowSeat>> GetShowSeatsAsync(
        int showId,
        List<int> seatIds);

    Task<Booking> CreateAsync(
        Booking booking,
        List<ShowSeat> showSeats);

    // Admin: Get booking by ID
    Task<Booking?> GetByIdAsync(int bookingId);

    // Customer: Get booking by ID only if it belongs to them
    Task<Booking?> GetByIdForUserAsync(
        int bookingId,
        int userId);

    // Admin: Get all bookings
    Task<List<Booking>> GetAllAsync(
        int? showId,
        string? status);

    // Admin: Get bookings by show
    Task<List<Booking>> GetByShowIdAsync(int showId);

    // Customer: Get their own bookings
    Task<List<Booking>> GetByUserIdAsync(int userId);

    // Admin: Cancel any booking
    Task<Booking?> CancelAsync(int bookingId);

    // Customer: Cancel only their own booking
    Task<Booking?> CancelForUserAsync(
        int bookingId,
        int userId);
}
