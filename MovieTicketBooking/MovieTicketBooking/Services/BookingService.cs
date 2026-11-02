
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

    // CREATE BOOKING
    public async Task<BookingResponse> CreateAsync(
        CreateBookingRequest request)
    {
        // Validate selected seats.
        if (request.SeatIds == null || request.SeatIds.Count == 0)
            throw new InvalidOperationException(
                "Select at least one seat.");

        if (request.SeatIds.Any(id => id <= 0))
            throw new InvalidOperationException(
                "Invalid seat ID.");

        // Prevent duplicate seat selection.
        if (request.SeatIds.Distinct().Count() != request.SeatIds.Count)
            throw new InvalidOperationException(
                "Duplicate seat IDs are not allowed.");

        // Check whether the show exists.
        var show = await _bookingRepository.GetShowByIdAsync(
            request.ShowId);

        if (show == null)
            throw new InvalidOperationException("Show not found.");

        // Retrieve selected seats belonging to this show.
        var showSeats = await _bookingRepository.GetShowSeatsAsync(
            request.ShowId,
            request.SeatIds);

        // Ensure every requested seat belongs to this show.
        if (showSeats.Count != request.SeatIds.Count)
            throw new InvalidOperationException(
                "One or more selected seats do not belong to this show.");

        // Check seat availability.
        if (showSeats.Any(seat => seat.Status != "Available"))
            throw new InvalidOperationException(
                "One or more selected seats are already booked.");

        // Calculate total amount using the server-side ticket price.
        decimal totalAmount = show.TicketPrice * showSeats.Count;

        // Create booking entity.
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

            // Create BookingSeat records.
            BookingSeats = showSeats.Select(showSeat =>
                new BookingSeat
                {
                    SeatId = showSeat.SeatId,
                    Seat = showSeat.Seat,
                    Price = show.TicketPrice
                }).ToList()
        };

        // Save booking and update seat availability in one transaction.
        var createdBooking = await _bookingRepository.CreateAsync(
            booking,
            showSeats);

        return MapToResponse(createdBooking);
    }

    // GET BOOKING BY ID
    public async Task<BookingResponse?> GetByIdAsync(int bookingId)
    {
        var booking = await _bookingRepository.GetByIdAsync(bookingId);

        if (booking == null)
            return null;

        return MapToResponse(booking);
    }

    // MAP BOOKING ENTITY TO RESPONSE DTO
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
                    SeatLabel = $"{seat.Seat.RowName}{seat.Seat.SeatNumber}",
                    Price = seat.Price
                }).ToList()
        };
    }
}
