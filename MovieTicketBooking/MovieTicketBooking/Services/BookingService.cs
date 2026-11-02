using MovieTicketBooking.DTOs.Bookings;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Services;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _bookingRepository;

    public BookingService(IBookingRepository bookingRepository)
    {
        _bookingRepository = bookingRepository;
    }

    // Create booking
    public async Task<BookingResponse> CreateAsync(
        CreateBookingRequest request)
    {
        if (request.SeatIds == null || request.SeatIds.Count == 0)
        {
            throw new InvalidOperationException(
                "Select at least one seat.");
        }

        if (request.SeatIds.Any(id => id <= 0))
        {
            throw new InvalidOperationException(
                "Invalid seat ID.");
        }

        if (request.SeatIds.Distinct().Count() != request.SeatIds.Count)
        {
            throw new InvalidOperationException(
                "Duplicate seat IDs are not allowed.");
        }

        var show = await _bookingRepository
            .GetShowByIdAsync(request.ShowId);

        if (show == null)
        {
            throw new InvalidOperationException("Show not found.");
        }

        var showSeats = await _bookingRepository
            .GetShowSeatsAsync(request.ShowId, request.SeatIds);

        if (showSeats.Count != request.SeatIds.Count)
        {
            throw new InvalidOperationException(
                "One or more selected seats do not belong to this show.");
        }

        if (showSeats.Any(seat => seat.Status != "Available"))
        {
            throw new InvalidOperationException(
                "One or more selected seats are already booked.");
        }

        decimal totalAmount = show.TicketPrice * showSeats.Count;

        var booking = new Booking
        {
            BookingReference = "MOV" +
                Guid.NewGuid().ToString("N")[..12].ToUpper(),

            ShowId = request.ShowId,
            CustomerName = request.CustomerName,
            CustomerEmail = request.CustomerEmail,
            CustomerPhone = request.CustomerPhone,
            BookingDate = DateTime.UtcNow,
            TotalAmount = totalAmount,
            Status = "Confirmed",

            BookingSeats = showSeats.Select(showSeat =>
                new BookingSeat
                {
                    SeatId = showSeat.SeatId,
                    Seat = showSeat.Seat,
                    Price = show.TicketPrice
                }).ToList()
        };

        var createdBooking = await _bookingRepository
            .CreateAsync(booking, showSeats);

        return MapToResponse(createdBooking);
    }

    // Get booking by ID
    public async Task<BookingResponse?> GetByIdAsync(int bookingId)
    {
        var booking = await _bookingRepository
            .GetByIdAsync(bookingId);

        if (booking == null)
        {
            return null;
        }

        return MapToResponse(booking);
    }

    // Get all bookings with optional filters
    public async Task<List<BookingResponse>> GetAllAsync(
        int? showId,
        string? status)
    {
        var bookings = await _bookingRepository
            .GetAllAsync(showId, status);

        return bookings
            .Select(MapToResponse)
            .ToList();
    }

    // Get bookings by show ID
    public async Task<List<BookingResponse>> GetByShowIdAsync(
        int showId)
    {
        var bookings = await _bookingRepository
            .GetByShowIdAsync(showId);

        return bookings
            .Select(MapToResponse)
            .ToList();
    }

    // Cancel booking
    public async Task<BookingResponse?> CancelAsync(int bookingId)
    {
        var existingBooking = await _bookingRepository
            .GetByIdAsync(bookingId);

        if (existingBooking == null)
        {
            return null;
        }

        if (existingBooking.Status != "Confirmed")
        {
            throw new InvalidOperationException(
                "Only confirmed bookings can be cancelled.");
        }

        var cancelledBooking = await _bookingRepository
            .CancelAsync(bookingId);

        if (cancelledBooking == null)
        {
            return null;
        }

        return MapToResponse(cancelledBooking);
    }

    // Convert Booking entity into BookingResponse DTO
    private static BookingResponse MapToResponse(Booking booking)
    {
        return new BookingResponse
        {
            BookingId = booking.BookingId,
            BookingReference = booking.BookingReference,
            ShowId = booking.ShowId,
            CustomerName = booking.CustomerName,
            CustomerEmail = booking.CustomerEmail,
            CustomerPhone = booking.CustomerPhone,
            BookingDate = booking.BookingDate,
            TotalAmount = booking.TotalAmount,
            Status = booking.Status,

            Seats = booking.BookingSeats.Select(seat =>
                new BookingSeatResponse
                {
                    SeatId = seat.SeatId,
                    RowName = seat.Seat.RowName,
                    SeatNumber = seat.Seat.SeatNumber,
                    SeatLabel =
                        $"{seat.Seat.RowName}{seat.Seat.SeatNumber}",
                    Price = seat.Price
                }).ToList()
        };
    }
}