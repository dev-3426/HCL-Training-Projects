
using MovieTicketBooking.DTOs.Screens;

namespace MovieTicketBooking.Services.Interfaces;

public interface IScreenService
{
    Task<ScreenResponse> CreateAsync(
        CreateScreenRequest request);

    Task<List<ScreenResponse>> GetAllAsync(int? theatreId);

    Task<ScreenResponse?> GetByIdAsync(int screenId);

    Task<ScreenResponse?> UpdateAsync(
        int screenId,
        UpdateScreenRequest request);

    Task<bool> DeleteAsync(int screenId);

    Task<List<ScreenResponse>> GetByTheatreIdAsync(
        int theatreId);
}
