using Microsoft.AspNetCore.Mvc;
using MovieTicketBooking.DTOs.Bookings;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Controllers;

[ApiController]
[Route("api/bookings")]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingsController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    // POST: api/bookings
    // Create booking
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateBookingRequest request)
    {
        var booking = await _bookingService.CreateAsync(request);

        return CreatedAtAction(
            nameof(GetById),
            new { id = booking.BookingId },
            booking);
    }

    // GET: api/bookings
    // Get all bookings with optional filters
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int? showId,
        [FromQuery] string? status)
    {
        var bookings = await _bookingService
            .GetAllAsync(showId, status);

        return Ok(bookings);
    }

    // GET: api/bookings/{id}
    // Get booking by ID
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var booking = await _bookingService.GetByIdAsync(id);

        if (booking == null)
        {
            return NotFound(new
            {
                message = "Booking not found."
            });
        }

        return Ok(booking);
    }

    // GET: api/bookings/show/{showId}
    // Get bookings for a particular show
    [HttpGet("show/{showId}")]
    public async Task<IActionResult> GetByShowId(int showId)
    {
        var bookings = await _bookingService
            .GetByShowIdAsync(showId);

        return Ok(bookings);
    }

    // PUT: api/bookings/{id}/cancel
    // Cancel booking
    [HttpPut("{id}/cancel")]
    public async Task<IActionResult> Cancel(int id)
    {
        var booking = await _bookingService.CancelAsync(id);

        if (booking == null)
        {
            return NotFound(new
            {
                message = "Booking not found."
            });
        }

        return Ok(booking);
    }
}