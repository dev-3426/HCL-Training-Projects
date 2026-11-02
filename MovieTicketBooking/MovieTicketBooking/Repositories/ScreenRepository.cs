
using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Data;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;

namespace MovieTicketBooking.Repositories;

public class ScreenRepository : IScreenRepository
{
    private readonly ApplicationDbContext _context;

    public ScreenRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    // Create screen
    public async Task<Screen> CreateAsync(Screen screen)
    {
        await _context.Screens.AddAsync(screen);
        await _context.SaveChangesAsync();

        return screen;
    }

    // Get all screens, optionally filtered by theatre
    public async Task<List<Screen>> GetAllAsync(int? theatreId)
    {
        var query = _context.Screens.AsNoTracking();

        if (theatreId.HasValue)
        {
            query = query.Where(screen =>
                screen.TheatreId == theatreId.Value);
        }

        return await query
            .OrderBy(screen => screen.ScreenId)
            .ToListAsync();
    }

    // Get screen by ID
    public async Task<Screen?> GetByIdAsync(int screenId)
    {
        return await _context.Screens
            .AsNoTracking()
            .FirstOrDefaultAsync(
                screen => screen.ScreenId == screenId);
    }

    // Update screen
    public async Task<Screen?> UpdateAsync(Screen screen)
    {
        var existingScreen = await _context.Screens
            .FirstOrDefaultAsync(
                s => s.ScreenId == screen.ScreenId);

        if (existingScreen == null)
            return null;

        existingScreen.TheatreId = screen.TheatreId;
        existingScreen.ScreenName = screen.ScreenName;

        await _context.SaveChangesAsync();

        return existingScreen;
    }

    // Delete screen
    public async Task<bool> DeleteAsync(int screenId)
    {
        var screen = await _context.Screens
            .FirstOrDefaultAsync(
                s => s.ScreenId == screenId);

        if (screen == null)
            return false;

        _context.Screens.Remove(screen);
        await _context.SaveChangesAsync();

        return true;
    }

    // Get screens belonging to a theatre
    public async Task<List<Screen>> GetByTheatreIdAsync(
        int theatreId)
    {
        return await _context.Screens
            .AsNoTracking()
            .Where(screen => screen.TheatreId == theatreId)
            .OrderBy(screen => screen.ScreenId)
            .ToListAsync();
    }
}
