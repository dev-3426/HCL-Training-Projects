
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieTicketBooking.Constants;
using MovieTicketBooking.DTOs.Shows;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Controllers;

[ApiController]
[Route("api/shows")]
public class ShowsController : ControllerBase
{
    private readonly IShowService _showService;

    public ShowsController(IShowService showService)
    {
        _showService = showService;
    }

    // POST: api/shows
    [Authorize(Roles = UserRoles.Admin)]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateShowRequest request)
    {
        var show = await _showService.CreateAsync(request);

        return CreatedAtAction(
            nameof(GetById),
            new { id = show.ShowId },
            show);
    }

    // GET: api/shows
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] DateTime? date,
        [FromQuery] int? movieId,
        [FromQuery] int? theatreId)
    {
        var shows = await _showService.GetAllAsync(
            date,
            movieId,
            theatreId);

        return Ok(shows);
    }

    // GET: api/shows/1
    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var show = await _showService.GetByIdAsync(id);

        if (show == null)
            return NotFound(new { message = "Show not found." });

        return Ok(show);
    }

    // GET: api/shows/1/seats
    [AllowAnonymous]
    [HttpGet("{showId}/seats")]
    public async Task<IActionResult> GetShowSeats(int showId)
    {
        var seats = await _showService.GetShowSeatsAsync(showId);

        return Ok(seats);
    }

    // GET: api/shows/1/seats/available
    [AllowAnonymous]
    [HttpGet("{showId}/seats/available")]
    public async Task<IActionResult> GetAvailableSeats(int showId)
    {
        var seats = await _showService.GetAvailableSeatsAsync(showId);

        return Ok(seats);
    }

    // GET: api/shows/1/seats/booked
    [AllowAnonymous]
    [HttpGet("{showId}/seats/booked")]
    public async Task<IActionResult> GetBookedSeats(int showId)
    {
        var seats = await _showService.GetBookedSeatsAsync(showId);

        return Ok(seats);
    }

    // DELETE: api/shows/1
    [Authorize(Roles = UserRoles.Admin)]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _showService.DeleteAsync(id);

        if (!deleted)
            return NotFound(new { message = "Show not found." });

        return NoContent();
    }

    // GET: api/shows/movie/1
    [AllowAnonymous]
    [HttpGet("movie/{movieId}")]
    public async Task<IActionResult> GetByMovieId(int movieId)
    {
        var shows = await _showService.GetByMovieIdAsync(movieId);

        return Ok(shows);
    }

    // GET: api/shows/theatre/1
    [AllowAnonymous]
    [HttpGet("theatre/{theatreId}")]
    public async Task<IActionResult> GetByTheatreId(int theatreId)
    {
        var shows = await _showService.GetByTheatreIdAsync(theatreId);

        return Ok(shows);
    }

    // GET: api/shows/search?date=2026-11-01
    [AllowAnonymous]
    [HttpGet("search")]
    public async Task<IActionResult> SearchByDate(
        [FromQuery] DateTime date)
    {
        var shows = await _showService.SearchByDateAsync(date);

        return Ok(shows);
    }

    // PUT: api/shows/1
    [Authorize(Roles = UserRoles.Admin)]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateShowRequest request)
    {
        var show = await _showService.UpdateAsync(id, request);

        if (show == null)
            return NotFound(new { message = "Show not found." });

        return Ok(show);
    }
}
