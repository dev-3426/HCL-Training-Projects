
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieTicketBooking.Constants;
using MovieTicketBooking.DTOs.Movies;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MoviesController : ControllerBase
{
    private readonly IMovieService _movieService;

    public MoviesController(IMovieService movieService)
    {
        _movieService = movieService;
    }

    // POST: api/movies
    [Authorize(Roles = UserRoles.Admin)]
    [HttpPost]
    public async Task<IActionResult> CreateMovie(
        CreateMovieRequest request)
    {
        var movie = await _movieService.CreateAsync(request);

        return CreatedAtAction(
            nameof(GetMovieById),
            new { id = movie.MovieId },
            movie);
    }

    // GET: api/movies
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAllMovies(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? title = null)
    {
        if (pageNumber < 1 || pageSize < 1 || pageSize > 100)
        {
            return BadRequest(
                "Page number must be at least 1 and page size must be between 1 and 100.");
        }

        var movies = await _movieService.GetAllAsync(
            pageNumber,
            pageSize,
            title);

        return Ok(movies);
    }

    // GET: api/movies/1
    [AllowAnonymous]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetMovieById(int id)
    {
        var movie = await _movieService.GetByIdAsync(id);

        if (movie == null)
        {
            return NotFound("Movie not found.");
        }

        return Ok(movie);
    }

    // PUT: api/movies/1
    [Authorize(Roles = UserRoles.Admin)]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateMovie(
        int id,
        UpdateMovieRequest request)
    {
        var movie = await _movieService.UpdateAsync(id, request);

        if (movie == null)
        {
            return NotFound("Movie not found.");
        }

        return Ok(movie);
    }

    // DELETE: api/movies/1
    [Authorize(Roles = UserRoles.Admin)]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteMovie(int id)
    {
        var deleted = await _movieService.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound("Movie not found.");
        }

        return NoContent();
    }

    // GET: api/movies/search?title=Leo
    [AllowAnonymous]
    [HttpGet("search")]
    public async Task<IActionResult> SearchMovies(
        [FromQuery] string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return BadRequest("Please provide a movie title.");
        }

        var movies = await _movieService.SearchByTitleAsync(title);

        return Ok(movies);
    }
}
