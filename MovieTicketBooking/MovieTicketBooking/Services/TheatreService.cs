
using MovieTicketBooking.DTOs.Theatres;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services.Interfaces;
using MovieTicketBooking.Exceptions;

namespace MovieTicketBooking.Services;

public class TheatreService : ITheatreService
{
    private readonly ITheatreRepository _theatreRepository;

    public TheatreService(ITheatreRepository theatreRepository)
    {
        _theatreRepository = theatreRepository;
    }

    // Create theatre
    public async Task<TheatreResponse> CreateAsync(
        CreateTheatreRequest request)
    {
        var theatre = new Theatre
        {
            Name = request.Name,
            Location = request.Location
        };

        var createdTheatre =
            await _theatreRepository.CreateAsync(theatre);

        return MapToResponse(createdTheatre);
    }

    // Get all theatres
    public async Task<List<TheatreResponse>> GetAllAsync()
    {
        var theatres = await _theatreRepository.GetAllAsync();

        return theatres.Select(MapToResponse).ToList();
    }

    // Get theatre by ID
    public async Task<TheatreResponse?> GetByIdAsync(int theatreId)
    {
        var theatre =
            await _theatreRepository.GetByIdAsync(theatreId);

        if (theatre == null)
            return null;

        return MapToResponse(theatre);
    }

    // Update theatre
    public async Task<TheatreResponse?> UpdateAsync(
        int theatreId,
        UpdateTheatreRequest request)
    {
        var theatre = new Theatre
        {
            TheatreId = theatreId,
            Name = request.Name,
            Location = request.Location
        };

        var updatedTheatre =
            await _theatreRepository.UpdateAsync(theatre);

        if (updatedTheatre == null)
        {
            throw new NotFoundException(
                "Theatre not found. Cannot update.");
        }

        return MapToResponse(updatedTheatre);
    }

    // Delete theatre
    public async Task<bool> DeleteAsync(int theatreId)
    {
        var isDeleted =
            await _theatreRepository.DeleteAsync(theatreId);

        if (!isDeleted)
        {
            throw new NotFoundException(
                "Theatre not found. Cannot delete.");
        }

        return true;
    }

    // Convert Entity into Response DTO
    private static TheatreResponse MapToResponse(Theatre theatre)
    {
        return new TheatreResponse
        {
            TheatreId = theatre.TheatreId,
            Name = theatre.Name,
            Location = theatre.Location
        };
    }
}
