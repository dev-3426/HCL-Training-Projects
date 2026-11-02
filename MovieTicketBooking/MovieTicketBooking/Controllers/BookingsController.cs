
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieTicketBooking.Constants;
using MovieTicketBooking.DTOs.Bookings;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingsController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    // Get logged-in user's ID from JWT claims
    private int GetCurrentUserId()
    {
        var userIdValue = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdValue, out int userId))
        {
            throw new UnauthorizedAccessException(
                "Invalid user authentication.");
        }

        return userId;
    }

    // Customer: Create booking
    [HttpPost]
    [Authorize(Roles = UserRoles.Customer)]
    public async Task<ActionResult<BookingResponse>> Create(
        CreateBookingRequest request)
    {
        int userId = GetCurrentUserId();

        var booking = await _bookingService.CreateAsync(
            request,
            userId);

        return CreatedAtAction(
            nameof(GetById),
            new { id = booking.BookingId },
            booking);
    }

    // Admin: Get all bookings
    [HttpGet]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<ActionResult<List<BookingResponse>>> GetAll(
        [FromQuery] int? showId,
        [FromQuery] string? status)
    {
        var bookings = await _bookingService.GetAllAsync(
            showId,
            status);

        return Ok(bookings);
    }

    // Customer: Get own booking history
    [HttpGet("my-bookings")]
    [Authorize(Roles = UserRoles.Customer)]
    public async Task<ActionResult<List<BookingResponse>>> GetMyBookings()
    {
        int userId = GetCurrentUserId();

        var bookings = await _bookingService.GetMyBookingsAsync(userId);

        return Ok(bookings);
    }

    // Admin: Get bookings by show
    [HttpGet("show/{showId:int}")]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<ActionResult<List<BookingResponse>>> GetByShowId(
        int showId)
    {
        var bookings = await _bookingService.GetByShowIdAsync(showId);

        return Ok(bookings);
    }

    // Admin or owning customer: Get booking by ID
    [HttpGet("{id:int}")]
    [Authorize(Roles = UserRoles.Admin + "," + UserRoles.Customer)]
    public async Task<ActionResult<BookingResponse>> GetById(int id)
    {
        if (User.IsInRole(UserRoles.Admin))
        {
            var adminBooking = await _bookingService.GetByIdAsync(id);

            if (adminBooking == null)
                return NotFound(new { message = "Booking not found." });

            return Ok(adminBooking);
        }

        int userId = GetCurrentUserId();

        var customerBooking = await _bookingService
            .GetByIdForUserAsync(id, userId);

        if (customerBooking == null)
            return NotFound(new { message = "Booking not found." });

        return Ok(customerBooking);
    }

    // Admin: Cancel any booking
    [HttpPut("{id:int}/cancel")]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<ActionResult<BookingResponse>> Cancel(int id)
    {
        var booking = await _bookingService.CancelAsync(id);

        if (booking == null)
            return NotFound(new { message = "Booking not found." });

        return Ok(booking);
    }

    // Customer: Cancel own booking
    [HttpPut("{id:int}/cancel-my-booking")]
    [Authorize(Roles = UserRoles.Customer)]
    public async Task<ActionResult<BookingResponse>> CancelMyBooking(int id)
    {
        int userId = GetCurrentUserId();

        var booking = await _bookingService.CancelForUserAsync(
            id,
            userId);

        if (booking == null)
            return NotFound(new { message = "Booking not found." });

        return Ok(booking);
    }
}
