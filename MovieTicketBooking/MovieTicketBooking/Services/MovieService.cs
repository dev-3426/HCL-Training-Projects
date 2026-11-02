
using MovieTicketBooking.DTOs.Movies;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services.Interfaces;
using MovieTicketBooking.Exceptions;

namespace MovieTicketBooking.Services;

public class MovieService : IMovieService
{
    private readonly IMovieRepository _movieRepository;

    public MovieService(IMovieRepository movieRepository)
    {
        _movieRepository = movieRepository;
    }

    // Create movie
    public async Task<MovieResponse> CreateAsync(
        CreateMovieRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
            throw new ValidationException("Movie title is required.");

        if (request.DurationMinutes <= 0)
            throw new ValidationException(
                "Movie duration must be greater than zero.");

        var movie = new Movie
        {
            Title = request.Title.Trim(),
            Description = request.Description?.Trim(),
            DurationMinutes = request.DurationMinutes,
            ReleaseDate = request.ReleaseDate,
            Language = request.Language.Trim(),
            Genre = request.Genre.Trim()
        };

        var createdMovie =
            await _movieRepository.CreateAsync(movie);

        return MapToResponse(createdMovie);
    }

    // Get all movies with pagination and optional title filter
    public async Task<List<MovieResponse>> GetAllAsync(
        int pageNumber,
        int pageSize,
        string? title)
    {
        if (pageNumber < 1)
            throw new ValidationException(
                "Page number must be greater than zero.");

        if (pageSize < 1 || pageSize > 100)
            throw new ValidationException(
                "Page size must be between 1 and 100.");

        var movies = await _movieRepository.GetAllAsync(
            pageNumber,
            pageSize,
            title?.Trim());

        return movies.Select(MapToResponse).ToList();
    }

    // Get movie by ID
    public async Task<MovieResponse?> GetByIdAsync(int movieId)
    {
        var movie = await _movieRepository.GetByIdAsync(movieId);

        if (movie == null)
            return null;

        return MapToResponse(movie);
    }

    // Update movie
    public async Task<MovieResponse?> UpdateAsync(
        int movieId,
        UpdateMovieRequest request)
    {
        var existingMovie = await _movieRepository.GetByIdAsync(movieId);

        if (existingMovie == null)
            throw new NotFoundException(
                "Movie not found. Cannot update.");

        if (string.IsNullOrWhiteSpace(request.Title))
            throw new ValidationException("Movie title is required.");

        if (request.DurationMinutes <= 0)
            throw new ValidationException(
                "Movie duration must be greater than zero.");

        var movie = new Movie
        {
            MovieId = movieId,
            Title = request.Title.Trim(),
            Description = request.Description?.Trim(),
            DurationMinutes = request.DurationMinutes,
            ReleaseDate = request.ReleaseDate,
            Language = request.Language.Trim(),
            Genre = request.Genre.Trim()
        };

        var updatedMovie =
            await _movieRepository.UpdateAsync(movie);

        if (updatedMovie == null)
            throw new NotFoundException(
                "Movie not found. Cannot update.");

        return MapToResponse(updatedMovie);
    }

    // Delete movie
    public async Task<bool> DeleteAsync(int movieId)
    {
        var movie = await _movieRepository.GetByIdAsync(movieId);

        if (movie == null)
            throw new NotFoundException(
                "Movie not found. Cannot delete.");

        bool isDeleted = await _movieRepository.DeleteAsync(movieId);

        if (!isDeleted)
            throw new ConflictException(
                "Movie cannot be deleted because it may have related shows.");

        return true;
    }

    // Search movies by title
    public async Task<List<MovieResponse>> SearchByTitleAsync(
        string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ValidationException(
                "Search title cannot be empty.");

        var movies =
            await _movieRepository.SearchByTitleAsync(title.Trim());

        return movies.Select(MapToResponse).ToList();
    }

    // Convert Movie entity into MovieResponse DTO
    private static MovieResponse MapToResponse(Movie movie)
    {
        return new MovieResponse
        {
            MovieId = movie.MovieId,
            Title = movie.Title,
            Description = movie.Description,
            DurationMinutes = movie.DurationMinutes,
            ReleaseDate = movie.ReleaseDate,
            Language = movie.Language,
            Genre = movie.Genre
        };
    }
}
