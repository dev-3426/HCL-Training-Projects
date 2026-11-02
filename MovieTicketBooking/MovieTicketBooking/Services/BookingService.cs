
using MovieTicketBooking.DTOs.Bookings;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services.Interfaces;
using MovieTicketBooking.Exceptions;

namespace MovieTicketBooking.Services;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _bookingRepository;

    public BookingService(IBookingRepository bookingRepository)
    {
        _bookingRepository = bookingRepository;
    }

    // Customer: Create booking
    public async Task<BookingResponse> CreateAsync(
        CreateBookingRequest request,
        int userId)
    {
        if (userId <= 0)
        {
            throw new ValidationException("Invalid user ID.");
        }

        // Validate selected seats
        if (request.SeatIds == null || request.SeatIds.Count == 0)
        {
            throw new ValidationException(
                "Select at least one seat.");
        }

        if (request.SeatIds.Any(id => id <= 0))
        {
            throw new ValidationException("Invalid seat ID.");
        }

        if (request.SeatIds.Distinct().Count() != request.SeatIds.Count)
        {
            throw new ValidationException(
                "Duplicate seat IDs are not allowed.");
        }

        // Validate show
        var show = await _bookingRepository
            .GetShowByIdAsync(request.ShowId);

        if (show == null)
        {
            throw new NotFoundException("Show not found.");
        }

        // Retrieve selected seats belonging to this show
        var showSeats = await _bookingRepository
            .GetShowSeatsAsync(request.ShowId, request.SeatIds);

        if (showSeats.Count != request.SeatIds.Count)
        {
            throw new ValidationException(
                "One or more selected seats do not belong to this show.");
        }

        // Check seat availability
        if (showSeats.Any(seat => seat.Status != "Available"))
        {
            throw new ConflictException(
                "One or more selected seats are already booked.");
        }

        // Calculate total booking amount
        decimal totalAmount = show.TicketPrice * showSeats.Count;

        // Create booking entity
        var booking = new Booking
        {
            BookingReference = "MOV" +
                Guid.NewGuid().ToString("N")[..12].ToUpper(),

            ShowId = request.ShowId,

            // Link booking to authenticated customer
            UserId = userId,

            CustomerName = request.CustomerName.Trim(),
            CustomerEmail = request.CustomerEmail.Trim(),
            CustomerPhone = request.CustomerPhone.Trim(),

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

    // Admin: Get booking by ID
    public async Task<BookingResponse?> GetByIdAsync(int bookingId)
    {
        var booking = await _bookingRepository
            .GetByIdAsync(bookingId);

        if (booking == null)
            return null;

        return MapToResponse(booking);
    }

    // Customer: Get booking by ID
    public async Task<BookingResponse?> GetByIdForUserAsync(
        int bookingId,
        int userId)
    {
        if (bookingId <= 0 || userId <= 0)
        {
            throw new ValidationException("Invalid booking or user ID.");
        }

        var booking = await _bookingRepository
            .GetByIdForUserAsync(bookingId, userId);

        if (booking == null)
            return null;

        return MapToResponse(booking);
    }

    // Admin: Get all bookings with optional filters
    public async Task<List<BookingResponse>> GetAllAsync(
        int? showId,
        string? status)
    {
        if (showId.HasValue && showId.Value <= 0)
        {
            throw new ValidationException(
                "Show ID must be greater than zero.");
        }

        var bookings = await _bookingRepository
            .GetAllAsync(showId, status?.Trim());

        return bookings
            .Select(MapToResponse)
            .ToList();
    }

    // Admin: Get bookings by show ID
    public async Task<List<BookingResponse>> GetByShowIdAsync(
        int showId)
    {
        if (showId <= 0)
        {
            throw new ValidationException(
                "Show ID must be greater than zero.");
        }

        var bookings = await _bookingRepository
            .GetByShowIdAsync(showId);

        return bookings
            .Select(MapToResponse)
            .ToList();
    }

    // Customer: Get their own bookings
    public async Task<List<BookingResponse>> GetMyBookingsAsync(int userId)
    {
        if (userId <= 0)
        {
            throw new ValidationException("Invalid user ID.");
        }

        var bookings = await _bookingRepository
            .GetByUserIdAsync(userId);

        return bookings
            .Select(MapToResponse)
            .ToList();
    }

    // Admin: Cancel any booking
    public async Task<BookingResponse?> CancelAsync(int bookingId)
    {
        if (bookingId <= 0)
        {
            throw new ValidationException(
                "Booking ID must be greater than zero.");
        }

        var existingBooking = await _bookingRepository
            .GetByIdAsync(bookingId);

        if (existingBooking == null)
        {
            throw new NotFoundException("Booking not found.");
        }

        if (existingBooking.Status != "Confirmed")
        {
            throw new ConflictException(
                "Only confirmed bookings can be cancelled.");
        }

        var cancelledBooking = await _bookingRepository
            .CancelAsync(bookingId);

        if (cancelledBooking == null)
        {
            throw new NotFoundException("Booking not found.");
        }

        return MapToResponse(cancelledBooking);
    }

    // Customer: Cancel only their own booking
    public async Task<BookingResponse?> CancelForUserAsync(
        int bookingId,
        int userId)
    {
        if (bookingId <= 0 || userId <= 0)
        {
            throw new ValidationException("Invalid booking or user ID.");
        }

        var existingBooking = await _bookingRepository
            .GetByIdForUserAsync(bookingId, userId);

        if (existingBooking == null)
        {
            throw new NotFoundException("Booking not found.");
        }

        if (existingBooking.Status != "Confirmed")
        {
            throw new ConflictException(
                "Only confirmed bookings can be cancelled.");
        }

        var cancelledBooking = await _bookingRepository
            .CancelForUserAsync(bookingId, userId);

        if (cancelledBooking == null)
        {
            throw new NotFoundException("Booking not found.");
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
