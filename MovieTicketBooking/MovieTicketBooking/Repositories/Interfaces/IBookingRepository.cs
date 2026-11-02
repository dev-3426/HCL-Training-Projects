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

    Task<Booking?> GetByIdAsync(int bookingId);

    Task<List<Booking>> GetAllAsync(
        int? showId,
        string? status);

    Task<List<Booking>> GetByShowIdAsync(int showId);

    Task<Booking?> CancelAsync(int bookingId);
}