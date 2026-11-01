
using MovieTicketBooking.DTOs.Movies;

namespace MovieTicketBooking.Services.Interfaces;

public interface IMovieService
{
    Task<MovieResponse> CreateAsync(CreateMovieRequest request);

    Task<List<MovieResponse>> GetAllAsync(
        int pageNumber,
        int pageSize,
        string? title);

    Task<MovieResponse?> GetByIdAsync(int movieId);

    Task<MovieResponse?> UpdateAsync(
        int movieId,
        UpdateMovieRequest request);

    Task<bool> DeleteAsync(int movieId);

    Task<List<MovieResponse>> SearchByTitleAsync(string title);
}
