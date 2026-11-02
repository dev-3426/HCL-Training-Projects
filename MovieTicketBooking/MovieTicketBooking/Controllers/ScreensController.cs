
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieTicketBooking.Constants;
using MovieTicketBooking.DTOs.Screens;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ScreensController : ControllerBase
{
    private readonly IScreenService _screenService;

    public ScreensController(IScreenService screenService)
    {
        _screenService = screenService;
    }

    // POST: api/screens
    [Authorize(Roles = UserRoles.Admin)]
    [HttpPost]
    public async Task<IActionResult> CreateScreen(
        CreateScreenRequest request)
    {
        var screen = await _screenService.CreateAsync(request);

        return CreatedAtAction(
            nameof(GetScreenById),
            new { id = screen.ScreenId },
            screen);
    }

    // GET: api/screens
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAllScreens(
        [FromQuery] int? theatreId)
    {
        var screens = await _screenService.GetAllAsync(theatreId);

        return Ok(screens);
    }

    // GET: api/screens/5
    [AllowAnonymous]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetScreenById(int id)
    {
        var screen = await _screenService.GetByIdAsync(id);

        if (screen == null)
            return NotFound("Screen not found.");

        return Ok(screen);
    }

    // GET: api/screens/theatre/1
    [AllowAnonymous]
    [HttpGet("theatre/{theatreId:int}")]
    public async Task<IActionResult> GetScreensByTheatre(int theatreId)
    {
        var screens = await _screenService.GetByTheatreIdAsync(theatreId);

        return Ok(screens);
    }

    // PUT: api/screens/5
    [Authorize(Roles = UserRoles.Admin)]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateScreen(
        int id,
        UpdateScreenRequest request)
    {
        var screen = await _screenService.UpdateAsync(id, request);

        if (screen == null)
            return NotFound("Screen not found.");

        return Ok(screen);
    }

    // DELETE: api/screens/5
    [Authorize(Roles = UserRoles.Admin)]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteScreen(int id)
    {
        var deleted = await _screenService.DeleteAsync(id);

        if (!deleted)
            return NotFound("Screen not found.");

        return NoContent();
    }
}
