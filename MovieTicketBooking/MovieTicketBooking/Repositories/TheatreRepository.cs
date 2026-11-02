
using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Data;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;

namespace MovieTicketBooking.Repositories;

public class TheatreRepository : ITheatreRepository
{
    private readonly ApplicationDbContext _context;

    public TheatreRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    // Create theatre
    public async Task<Theatre> CreateAsync(Theatre theatre)
    {
        await _context.Theatres.AddAsync(theatre);
        await _context.SaveChangesAsync();

        return theatre;
    }

    // Get all theatres
    public async Task<List<Theatre>> GetAllAsync()
    {
        return await _context.Theatres
            .AsNoTracking()
            .ToListAsync();
    }

    // Get theatre by ID
    public async Task<Theatre?> GetByIdAsync(int theatreId)
    {
        return await _context.Theatres
            .AsNoTracking()
            .FirstOrDefaultAsync(
                theatre => theatre.TheatreId == theatreId);
    }

    // Update theatre
    public async Task<Theatre?> UpdateAsync(Theatre theatre)
    {
        var existingTheatre = await _context.Theatres
            .FirstOrDefaultAsync(
                t => t.TheatreId == theatre.TheatreId);

        if (existingTheatre == null)
            return null;

        existingTheatre.Name = theatre.Name;
        existingTheatre.Location = theatre.Location;

        await _context.SaveChangesAsync();

        return existingTheatre;
    }

    // Delete theatre
    public async Task<bool> DeleteAsync(int theatreId)
    {
        var theatre = await _context.Theatres
            .FirstOrDefaultAsync(
                t => t.TheatreId == theatreId);

        if (theatre == null)
            return false;

        _context.Theatres.Remove(theatre);

        await _context.SaveChangesAsync();

        return true;
    }
}
