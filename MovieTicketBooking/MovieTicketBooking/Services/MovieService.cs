
using MovieTicketBooking.DTOs.Movies;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Services;

public class MovieService : IMovieService
{
    private readonly IMovieRepository _movieRepository;

    public MovieService(IMovieRepository movieRepository)
    {
        _movieRepository = movieRepository;
    }

    public async Task<MovieResponse> CreateAsync(
        CreateMovieRequest request)
    {
        var movie = new Movie
        {
            Title = request.Title,
            Description = request.Description,
            DurationMinutes = request.DurationMinutes,
            ReleaseDate = request.ReleaseDate,
            Language = request.Language,
            Genre = request.Genre
        };

        var createdMovie =
            await _movieRepository.CreateAsync(movie);

        return MapToResponse(createdMovie);
    }


    public async Task<List<MovieResponse>> GetAllAsync(
        int pageNumber,
        int pageSize,
        string? title)
    {
        var movies = await _movieRepository.GetAllAsync(
            pageNumber,
            pageSize,
            title);

        return movies.Select(MapToResponse).ToList();
    }


    public async Task<MovieResponse?> GetByIdAsync(int movieId)
    {
        var movie = await _movieRepository.GetByIdAsync(movieId);

        if (movie == null)
        {
            return null;
        }

        return MapToResponse(movie);
    }

    public async Task<MovieResponse?> UpdateAsync(
        int movieId,
        UpdateMovieRequest request)
    {
        var movie = new Movie
        {
            MovieId = movieId,
            Title = request.Title,
            Description = request.Description,
            DurationMinutes = request.DurationMinutes,
            ReleaseDate = request.ReleaseDate,
            Language = request.Language,
            Genre = request.Genre
        };

        var updatedMovie =
            await _movieRepository.UpdateAsync(movie);

        if (updatedMovie == null)
        {
            return null;
        }

        return MapToResponse(updatedMovie);
    }

    public async Task<bool> DeleteAsync(int movieId)
    {
        return await _movieRepository.DeleteAsync(movieId);
    }

    public async Task<List<MovieResponse>> SearchByTitleAsync(
        string title)
    {
        var movies =
            await _movieRepository.SearchByTitleAsync(title);

        return movies.Select(MapToResponse).ToList();
    }

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
