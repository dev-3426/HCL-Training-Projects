
using MovieTicketBooking.Models;

namespace MovieTicketBooking.Repositories.Interfaces;

public interface IMovieRepository
{
    Task<Movie> CreateAsync(Movie movie);

    Task<List<Movie>> GetAllAsync(
    int pageNumber,
    int pageSize,
    string? title);

    Task<Movie?> GetByIdAsync(int movieId);

    Task<Movie?> UpdateAsync(Movie movie);

    Task<bool> DeleteAsync(int movieId);

    Task<List<Movie>> SearchByTitleAsync(string title);
}
