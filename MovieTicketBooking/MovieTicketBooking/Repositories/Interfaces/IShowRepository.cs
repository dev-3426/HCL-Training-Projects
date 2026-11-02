using MovieTicketBooking.Models;

namespace MovieTicketBooking.Repositories.Interfaces;

public interface IShowRepository
{
    Task<Show> CreateAsync(Show show);

    Task CreateShowSeatsAsync(List<ShowSeat> showSeats);

    Task<List<ShowSeat>> GetShowSeatsAsync(int showId);

    Task<List<Show>> GetAllAsync(
        DateTime? date,
        int? movieId,
        int? theatreId);

    Task<Show?> GetByIdAsync(int showId);

    Task<List<Show>> GetByMovieIdAsync(int movieId);

    Task<List<Show>> GetByTheatreIdAsync(int theatreId);

    Task<List<Show>> SearchByDateAsync(DateTime date);

    Task<Show?> UpdateAsync(Show show);

    Task<bool> DeleteAsync(int showId);

    Task<bool> HasOverlapAsync(
        int screenId,
        DateTime startTime,
        DateTime endTime,
        int? excludeShowId = null);

    Task<List<Seat>> GetSeatsByScreenIdAsync(int screenId);
}