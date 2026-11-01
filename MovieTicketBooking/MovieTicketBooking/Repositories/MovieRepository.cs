
using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Data;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;

namespace MovieTicketBooking.Repositories;

public class MovieRepository : IMovieRepository
{
    private readonly ApplicationDbContext _context;

    public MovieRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Movie> CreateAsync(Movie movie)
    {
        await _context.Movies.AddAsync(movie);

        await _context.SaveChangesAsync();

        return movie;
    }
    public async Task<List<Movie>> GetAllAsync(
        int pageNumber,
        int pageSize,
        string? title)
    {
        var query = _context.Movies.AsNoTracking();

        // Apply title filtering when a title is provided
        if (!string.IsNullOrWhiteSpace(title))
        {
            query = query.Where(movie =>
                movie.Title.Contains(title));
        }

        // Apply pagination
        return await query
            .OrderBy(movie => movie.MovieId)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }


    public async Task<Movie?> GetByIdAsync(int movieId)
    {
        return await _context.Movies
            .AsNoTracking()
            .FirstOrDefaultAsync(movie => movie.MovieId == movieId);
    }

    public async Task<Movie?> UpdateAsync(Movie movie)
    {
        var existingMovie = await _context.Movies
            .FirstOrDefaultAsync(m => m.MovieId == movie.MovieId);

        if (existingMovie == null)
        {
            return null;
        }

        existingMovie.Title = movie.Title;
        existingMovie.Description = movie.Description;
        existingMovie.DurationMinutes = movie.DurationMinutes;
        existingMovie.ReleaseDate = movie.ReleaseDate;
        existingMovie.Language = movie.Language;
        existingMovie.Genre = movie.Genre;

        await _context.SaveChangesAsync();

        return existingMovie;
    }

    public async Task<bool> DeleteAsync(int movieId)
    {
        var movie = await _context.Movies
            .FirstOrDefaultAsync(m => m.MovieId == movieId);

        if (movie == null)
        {
            return false;
        }

        _context.Movies.Remove(movie);

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<List<Movie>> SearchByTitleAsync(string title)
    {
        return await _context.Movies
            .AsNoTracking()
            .Where(movie => movie.Title.Contains(title))
            .ToListAsync();
    }
}
