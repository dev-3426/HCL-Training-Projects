
using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Data;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;

namespace MovieTicketBooking.Repositories;

public class BookingRepository : IBookingRepository
{
    private readonly ApplicationDbContext _context;

    public BookingRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    // Get show details
    public async Task<Show?> GetShowByIdAsync(int showId)
    {
        return await _context.Shows
            .FirstOrDefaultAsync(s => s.ShowId == showId);
    }

    // Get selected seats for a particular show
    public async Task<List<ShowSeat>> GetShowSeatsAsync(
        int showId,
        List<int> seatIds)
    {
        return await _context.ShowSeats
            .Include(ss => ss.Seat)
            .Where(ss =>
                ss.ShowId == showId &&
                seatIds.Contains(ss.SeatId))
            .ToListAsync();
    }

    // Create booking and update seat status
    public async Task<Booking> CreateAsync(
        Booking booking,
        List<ShowSeat> showSeats)
    {
        await using var transaction =
            await _context.Database.BeginTransactionAsync();

        try
        {
            foreach (var showSeat in showSeats)
            {
                showSeat.Status = "Booked";
            }

            await _context.Bookings.AddAsync(booking);

            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            return booking;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    // Admin: Get booking by ID
    public async Task<Booking?> GetByIdAsync(int bookingId)
    {
        return await _context.Bookings
            .AsNoTracking()
            .Include(b => b.BookingSeats)
                .ThenInclude(bs => bs.Seat)
            .FirstOrDefaultAsync(b => b.BookingId == bookingId);
    }

    // Customer: Get booking by ID only if it belongs to them
    public async Task<Booking?> GetByIdForUserAsync(
        int bookingId,
        int userId)
    {
        return await _context.Bookings
            .AsNoTracking()
            .Include(b => b.BookingSeats)
                .ThenInclude(bs => bs.Seat)
            .FirstOrDefaultAsync(b =>
                b.BookingId == bookingId &&
                b.UserId == userId);
    }

    // Admin: Get all bookings with optional filters
    public async Task<List<Booking>> GetAllAsync(
        int? showId,
        string? status)
    {
        var query = _context.Bookings
            .AsNoTracking()
            .Include(b => b.BookingSeats)
                .ThenInclude(bs => bs.Seat)
            .AsQueryable();

        if (showId.HasValue)
        {
            query = query.Where(b => b.ShowId == showId.Value);
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(b => b.Status == status);
        }

        return await query
            .OrderByDescending(b => b.BookingDate)
            .ToListAsync();
    }

    // Admin: Get bookings for a particular show
    public async Task<List<Booking>> GetByShowIdAsync(int showId)
    {
        return await _context.Bookings
            .AsNoTracking()
            .Include(b => b.BookingSeats)
                .ThenInclude(bs => bs.Seat)
            .Where(b => b.ShowId == showId)
            .OrderByDescending(b => b.BookingDate)
            .ToListAsync();
    }

    // Customer: Get only their own bookings
    public async Task<List<Booking>> GetByUserIdAsync(int userId)
    {
        return await _context.Bookings
            .AsNoTracking()
            .Include(b => b.BookingSeats)
                .ThenInclude(bs => bs.Seat)
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.BookingDate)
            .ToListAsync();
    }

    // Admin: Cancel any booking
    public async Task<Booking?> CancelAsync(int bookingId)
    {
        return await CancelBookingInternalAsync(bookingId, null);
    }

    // Customer: Cancel only their own booking
    public async Task<Booking?> CancelForUserAsync(
        int bookingId,
        int userId)
    {
        return await CancelBookingInternalAsync(bookingId, userId);
    }

    // Shared cancellation logic
    private async Task<Booking?> CancelBookingInternalAsync(
        int bookingId,
        int? userId)
    {
        await using var transaction =
            await _context.Database.BeginTransactionAsync();

        try
        {
            var query = _context.Bookings
                .Include(b => b.BookingSeats)
                    .ThenInclude(bs => bs.Seat)
                .Where(b => b.BookingId == bookingId);

            // Apply ownership filter for customer cancellation
            if (userId.HasValue)
            {
                query = query.Where(b => b.UserId == userId.Value);
            }

            var booking = await query.FirstOrDefaultAsync();

            if (booking == null)
            {
                await transaction.RollbackAsync();
                return null;
            }

            // Update booking status
            booking.Status = "Cancelled";

            // Get the corresponding ShowSeat records
            var seatIds = booking.BookingSeats
                .Select(bs => bs.SeatId)
                .ToList();

            var showSeats = await _context.ShowSeats
                .Where(ss =>
                    ss.ShowId == booking.ShowId &&
                    seatIds.Contains(ss.SeatId))
                .ToListAsync();

            // Release the seats
            foreach (var showSeat in showSeats)
            {
                showSeat.Status = "Available";
            }

            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            return booking;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
