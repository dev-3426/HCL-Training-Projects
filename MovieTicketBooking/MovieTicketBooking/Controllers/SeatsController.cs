
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieTicketBooking.Constants;
using MovieTicketBooking.DTOs.Seats;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SeatsController : ControllerBase
{
    private readonly ISeatService _seatService;

    public SeatsController(ISeatService seatService)
    {
        _seatService = seatService;
    }

    // POST: api/seats
    [Authorize(Roles = UserRoles.Admin)]
    [HttpPost]
    public async Task<IActionResult> CreateSeat(
        CreateSeatRequest request)
    {
        var seat = await _seatService.CreateAsync(request);

        return CreatedAtAction(
            nameof(GetSeatById),
            new { id = seat.SeatId },
            seat);
    }

    // POST: api/seats/bulk
    [Authorize(Roles = UserRoles.Admin)]
    [HttpPost("bulk")]
    public async Task<IActionResult> CreateBulkSeats(
        BulkCreateSeatsRequest request)
    {
        var seats = await _seatService.CreateBulkAsync(request);

        return StatusCode(StatusCodes.Status201Created, seats);
    }

    // GET: api/seats/screen/1
    [AllowAnonymous]
    [HttpGet("screen/{screenId:int}")]
    public async Task<IActionResult> GetSeatsByScreen(int screenId)
    {
        var seats = await _seatService.GetByScreenIdAsync(screenId);

        return Ok(seats);
    }

    // GET: api/seats/5
    [AllowAnonymous]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetSeatById(int id)
    {
        var seat = await _seatService.GetByIdAsync(id);

        if (seat == null)
            return NotFound("Seat not found.");

        return Ok(seat);
    }

    // PUT: api/seats/5
    [Authorize(Roles = UserRoles.Admin)]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateSeat(
        int id,
        UpdateSeatRequest request)
    {
        var seat = await _seatService.UpdateAsync(id, request);

        if (seat == null)
            return NotFound("Seat not found.");

        return Ok(seat);
    }

    // DELETE: api/seats/5
    [Authorize(Roles = UserRoles.Admin)]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteSeat(int id)
    {
        var deleted = await _seatService.DeleteAsync(id);

        if (!deleted)
            return NotFound("Seat not found.");

        return NoContent();
    }
}
