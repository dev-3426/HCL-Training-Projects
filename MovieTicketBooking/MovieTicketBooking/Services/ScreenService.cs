
using MovieTicketBooking.DTOs.Screens;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services.Interfaces;
using MovieTicketBooking.Exceptions;

namespace MovieTicketBooking.Services;

public class ScreenService : IScreenService
{
    private readonly IScreenRepository _screenRepository;
    private readonly ITheatreRepository _theatreRepository;

    public ScreenService(
        IScreenRepository screenRepository,
        ITheatreRepository theatreRepository)
    {
        _screenRepository = screenRepository;
        _theatreRepository = theatreRepository;
    }

    // Create screen
    public async Task<ScreenResponse> CreateAsync(CreateScreenRequest request)
    {
        var theatre = await _theatreRepository.GetByIdAsync(
            request.TheatreId);

        if (theatre == null)
            throw new NotFoundException("Theatre not found.");

        var screen = new Screen
        {
            TheatreId = request.TheatreId,
            ScreenName = request.ScreenName.Trim()
        };

        var createdScreen = await _screenRepository.CreateAsync(screen);

        return MapToResponse(createdScreen);
    }

    // Get all screens
    public async Task<List<ScreenResponse>> GetAllAsync(int? theatreId)
    {
        if (theatreId.HasValue)
        {
            var theatre = await _theatreRepository.GetByIdAsync(
                theatreId.Value);

            if (theatre == null)
                throw new NotFoundException("Theatre not found.");
        }

        var screens = await _screenRepository.GetAllAsync(theatreId);

        return screens.Select(MapToResponse).ToList();
    }

    // Get screen by ID
    public async Task<ScreenResponse?> GetByIdAsync(int screenId)
    {
        var screen = await _screenRepository.GetByIdAsync(screenId);

        if (screen == null)
            return null;

        return MapToResponse(screen);
    }

    // Update screen
    public async Task<ScreenResponse?> UpdateAsync(
        int screenId,
        UpdateScreenRequest request)
    {
        var existingScreen = await _screenRepository.GetByIdAsync(screenId);

        if (existingScreen == null)
            throw new NotFoundException(
                "Screen not found. Cannot update.");

        var theatre = await _theatreRepository.GetByIdAsync(
            request.TheatreId);

        if (theatre == null)
            throw new NotFoundException("Theatre not found.");

        var screen = new Screen
        {
            ScreenId = screenId,
            TheatreId = request.TheatreId,
            ScreenName = request.ScreenName.Trim()
        };

        var updatedScreen = await _screenRepository.UpdateAsync(screen);

        if (updatedScreen == null)
            throw new NotFoundException(
                "Screen not found. Cannot update.");

        return MapToResponse(updatedScreen);
    }

    // Delete screen
    public async Task<bool> DeleteAsync(int screenId)
    {
        var screen = await _screenRepository.GetByIdAsync(screenId);

        if (screen == null)
            throw new NotFoundException(
                "Screen not found. Cannot delete.");

        bool isDeleted = await _screenRepository.DeleteAsync(screenId);

        if (!isDeleted)
            throw new ConflictException(
                "Screen could not be deleted. It may have related seats or shows.");

        return true;
    }

    // Get screens by theatre
    public async Task<List<ScreenResponse>> GetByTheatreIdAsync(int theatreId)
    {
        var theatre = await _theatreRepository.GetByIdAsync(theatreId);

        if (theatre == null)
            throw new NotFoundException("Theatre not found.");

        var screens = await _screenRepository.GetByTheatreIdAsync(theatreId);

        return screens.Select(MapToResponse).ToList();
    }

    // Convert Screen entity into ScreenResponse DTO
    private static ScreenResponse MapToResponse(Screen screen)
    {
        return new ScreenResponse
        {
            ScreenId = screen.ScreenId,
            TheatreId = screen.TheatreId,
            ScreenName = screen.ScreenName
        };
    }
}
