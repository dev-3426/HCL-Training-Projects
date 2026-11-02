using MovieTicketBooking.DTOs.Screens;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Services;

public class ScreenService : IScreenService
{
    private readonly IScreenRepository _screenRepository;

    public ScreenService(IScreenRepository screenRepository)
    {
        _screenRepository = screenRepository;
    }

    public async Task<ScreenResponse> CreateAsync(CreateScreenRequest request)
    {
        var screen = new Screen
        {
            TheatreId = request.TheatreId,
            ScreenName = request.ScreenName
        };

        var createdScreen = await _screenRepository.CreateAsync(screen);

        return MapToResponse(createdScreen);
    }

    public async Task<List<ScreenResponse>> GetAllAsync(int? theatreId)
    {
        var screens = await _screenRepository.GetAllAsync(theatreId);

        return screens.Select(MapToResponse).ToList();
    }

    public async Task<ScreenResponse?> GetByIdAsync(int screenId)
    {
        var screen = await _screenRepository.GetByIdAsync(screenId);

        if (screen == null)
            return null;

        return MapToResponse(screen);
    }

    public async Task<ScreenResponse?> UpdateAsync(
        int screenId,
        UpdateScreenRequest request)
    {
        var screen = new Screen
        {
            ScreenId = screenId,
            TheatreId = request.TheatreId,
            ScreenName = request.ScreenName
        };

        var updatedScreen = await _screenRepository.UpdateAsync(screen);

        if (updatedScreen == null)
            return null;

        return MapToResponse(updatedScreen);
    }

    public async Task<bool> DeleteAsync(int screenId)
    {
        return await _screenRepository.DeleteAsync(screenId);
    }

    public async Task<List<ScreenResponse>> GetByTheatreIdAsync(int theatreId)
    {
        var screens = await _screenRepository.GetByTheatreIdAsync(theatreId);

        return screens.Select(MapToResponse).ToList();
    }

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