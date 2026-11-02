using MovieTicketBooking.Models;

namespace MovieTicketBooking.Repositories.Interfaces;

public interface ISeatRepository
{
    Task<Seat> CreateAsync(Seat seat);

    Task<List<Seat>> CreateBulkAsync(List<Seat> seats);

    Task<List<Seat>> GetByScreenIdAsync(int screenId);

    Task<Seat?> GetByIdAsync(int seatId);

    Task<Seat?> UpdateAsync(Seat seat);

    Task<bool> DeleteAsync(int seatId);

    Task<bool> ExistsAsync(
        int screenId,
        string rowName,
        int seatNumber);
}