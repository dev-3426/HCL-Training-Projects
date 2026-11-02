using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Data;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;

namespace MovieTicketBooking.Repositories;

public class SeatRepository : ISeatRepository
{
    private readonly ApplicationDbContext _context;

    public SeatRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    // Create one seat
    public async Task<Seat> CreateAsync(Seat seat)
    {
        await _context.Seats.AddAsync(seat);

        await _context.SaveChangesAsync();

        return seat;
    }

    // Create multiple seats
    public async Task<List<Seat>> CreateBulkAsync(List<Seat> seats)
    {
        await _context.Seats.AddRangeAsync(seats);

        await _context.SaveChangesAsync();

        return seats;
    }

    // Get all seats belonging to a screen
    public async Task<List<Seat>> GetByScreenIdAsync(int screenId)
    {
        return await _context.Seats
            .AsNoTracking()
            .Where(seat => seat.ScreenId == screenId)
            .OrderBy(seat => seat.RowName)
            .ThenBy(seat => seat.SeatNumber)
            .ToListAsync();
    }

    // Get seat by ID
    public async Task<Seat?> GetByIdAsync(int seatId)
    {
        return await _context.Seats
            .AsNoTracking()
            .FirstOrDefaultAsync(seat => seat.SeatId == seatId);
    }

    // Update seat
    public async Task<Seat?> UpdateAsync(Seat seat)
    {
        var existingSeat = await _context.Seats
            .FirstOrDefaultAsync(s => s.SeatId == seat.SeatId);

        if (existingSeat == null)
            return null;

        existingSeat.ScreenId = seat.ScreenId;
        existingSeat.RowName = seat.RowName;
        existingSeat.SeatNumber = seat.SeatNumber;
        existingSeat.SeatType = seat.SeatType;

        await _context.SaveChangesAsync();

        return existingSeat;
    }

    // Delete seat
    public async Task<bool> DeleteAsync(int seatId)
    {
        var seat = await _context.Seats
            .FirstOrDefaultAsync(s => s.SeatId == seatId);

        if (seat == null)
            return false;

        _context.Seats.Remove(seat);

        await _context.SaveChangesAsync();

        return true;
    }

    // Check duplicate seat position
    public async Task<bool> ExistsAsync(
        int screenId,
        string rowName,
        int seatNumber)
    {
        return await _context.Seats.AnyAsync(seat =>
            seat.ScreenId == screenId &&
            seat.RowName == rowName &&
            seat.SeatNumber == seatNumber);
    }
}