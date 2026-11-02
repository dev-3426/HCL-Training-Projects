using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Data;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;

namespace MovieTicketBooking.Repositories;

public class ShowRepository : IShowRepository
{
    private readonly ApplicationDbContext _context;

    public ShowRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    // Create a new show
    public async Task<Show> CreateAsync(Show show)
    {
        await _context.Shows.AddAsync(show);

        await _context.SaveChangesAsync();

        return show;
    }

    public async Task CreateShowSeatsAsync(List<ShowSeat> showSeats)
    {
        await _context.ShowSeats.AddRangeAsync(showSeats);

        await _context.SaveChangesAsync();
    }

    public async Task<List<ShowSeat>> GetShowSeatsAsync(int showId)
    {
        return await _context.ShowSeats
            .Include(showSeat => showSeat.Seat)
            .Where(showSeat => showSeat.ShowId == showId)
            .ToListAsync();
    }

    public async Task<bool> DeleteAsync(int showId)
    {
        var show = await _context.Shows
            .FirstOrDefaultAsync(s => s.ShowId == showId);

        if (show == null)
            return false;

        _context.Shows.Remove(show);

        await _context.SaveChangesAsync();

        return true;
    }


    // Get all shows with optional filters
    public async Task<List<Show>> GetAllAsync(
        DateTime? date,
        int? movieId,
        int? theatreId)
    {
        var query = _context.Shows
            .AsNoTracking()
            .Include(show => show.Screen)
                .ThenInclude(screen => screen.Theatre)
            .AsQueryable();

        if (date.HasValue)
        {
            var startDate = date.Value.Date;
            var endDate = startDate.AddDays(1);

            query = query.Where(show =>
                show.StartTime >= startDate &&
                show.StartTime < endDate);
        }

        if (movieId.HasValue)
        {
            query = query.Where(show =>
                show.MovieId == movieId.Value);
        }

        if (theatreId.HasValue)
        {
            query = query.Where(show =>
                show.Screen.TheatreId == theatreId.Value);
        }

        return await query
            .OrderBy(show => show.StartTime)
            .ToListAsync();
    }

    // Get show by ID
    public async Task<Show?> GetByIdAsync(int showId)
    {
        return await _context.Shows
            .AsNoTracking()
            .FirstOrDefaultAsync(show =>
                show.ShowId == showId);
    }

    // Get shows belonging to a movie
    public async Task<List<Show>> GetByMovieIdAsync(int movieId)
    {
        return await _context.Shows
            .AsNoTracking()
            .Where(show => show.MovieId == movieId)
            .OrderBy(show => show.StartTime)
            .ToListAsync();
    }

    // Get shows belonging to a theatre
    public async Task<List<Show>> GetByTheatreIdAsync(int theatreId)
    {
        return await _context.Shows
            .AsNoTracking()
            .Where(show => show.Screen.TheatreId == theatreId)
            .OrderBy(show => show.StartTime)
            .ToListAsync();
    }

    // Search shows by date
    public async Task<List<Show>> SearchByDateAsync(DateTime date)
    {
        var startDate = date.Date;
        var endDate = startDate.AddDays(1);

        return await _context.Shows
            .AsNoTracking()
            .Where(show =>
                show.StartTime >= startDate &&
                show.StartTime < endDate)
            .OrderBy(show => show.StartTime)
            .ToListAsync();
    }

    // Update an existing show
    public async Task<Show?> UpdateAsync(Show show)
    {
        var existingShow = await _context.Shows
            .FirstOrDefaultAsync(s =>
                s.ShowId == show.ShowId);

        if (existingShow == null)
            return null;

        existingShow.MovieId = show.MovieId;
        existingShow.ScreenId = show.ScreenId;
        existingShow.StartTime = show.StartTime;
        existingShow.EndTime = show.EndTime;
        existingShow.TicketPrice = show.TicketPrice;

        await _context.SaveChangesAsync();

        return existingShow;
    }

    // Check whether another show overlaps
    public async Task<bool> HasOverlapAsync(
        int screenId,
        DateTime startTime,
        DateTime endTime,
        int? excludeShowId = null)
    {
        var query = _context.Shows
            .Where(show =>
                show.ScreenId == screenId &&
                show.StartTime < endTime &&
                show.EndTime > startTime);

        if (excludeShowId.HasValue)
        {
            query = query.Where(show =>
                show.ShowId != excludeShowId.Value);
        }

        return await query.AnyAsync();
    }

    // Get physical seats belonging to a screen
    public async Task<List<Seat>> GetSeatsByScreenIdAsync(int screenId)
    {
        return await _context.Seats
            .AsNoTracking()
            .Where(seat => seat.ScreenId == screenId)
            .OrderBy(seat => seat.RowName)
            .ThenBy(seat => seat.SeatNumber)
            .ToListAsync();
    }
}