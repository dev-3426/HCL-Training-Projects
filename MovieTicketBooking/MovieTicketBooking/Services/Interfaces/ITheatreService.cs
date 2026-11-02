
using MovieTicketBooking.DTOs.Theatres;

namespace MovieTicketBooking.Services.Interfaces;

public interface ITheatreService
{
    Task<TheatreResponse> CreateAsync(
        CreateTheatreRequest request);

    Task<List<TheatreResponse>> GetAllAsync();

    Task<TheatreResponse?> GetByIdAsync(int theatreId);

    Task<TheatreResponse?> UpdateAsync(
        int theatreId,
        UpdateTheatreRequest request);

    Task<bool> DeleteAsync(int theatreId);
}
