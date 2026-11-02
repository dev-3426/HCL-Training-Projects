using MovieTicketBooking.DTOs.Shows;

namespace MovieTicketBooking.Services.Interfaces;

public interface IShowService
{
    Task<ShowResponse> CreateAsync(CreateShowRequest request);

    Task<List<ShowResponse>> GetAllAsync(
        DateTime? date, int? movieId, int? theatreId);

    Task<ShowResponse?> GetByIdAsync(int showId);

    Task<List<ShowResponse>> GetByMovieIdAsync(int movieId);

    Task<List<ShowResponse>> GetByTheatreIdAsync(int theatreId);

    Task<List<ShowResponse>> SearchByDateAsync(DateTime date);

    Task<ShowResponse?> UpdateAsync(
        int showId, UpdateShowRequest request);

    Task<List<ShowSeatResponse>> GetShowSeatsAsync(int showId);

    Task<List<ShowSeatResponse>> GetAvailableSeatsAsync(int showId);

    Task<List<ShowSeatResponse>> GetBookedSeatsAsync(int showId);

    Task<bool> DeleteAsync(int showId);
}