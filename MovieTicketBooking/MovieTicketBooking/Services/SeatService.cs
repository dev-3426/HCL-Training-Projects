using MovieTicketBooking.DTOs.Seats;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Services;

public class SeatService : ISeatService
{
    private readonly ISeatRepository _seatRepository;
    private readonly IScreenRepository _screenRepository;

    public SeatService(
        ISeatRepository seatRepository,
        IScreenRepository screenRepository)
    {
        _seatRepository = seatRepository;
        _screenRepository = screenRepository;
    }

    // Create one seat
    public async Task<SeatResponse> CreateAsync(
        CreateSeatRequest request)
    {
        var screen = await _screenRepository.GetByIdAsync(
            request.ScreenId);

        if (screen == null)
            throw new InvalidOperationException("Screen not found.");

        var exists = await _seatRepository.ExistsAsync(
            request.ScreenId,
            request.RowName,
            request.SeatNumber);

        if (exists)
            throw new InvalidOperationException(
                "This seat position already exists.");

        var seat = new Seat
        {
            ScreenId = request.ScreenId,
            RowName = request.RowName,
            SeatNumber = request.SeatNumber,
            SeatType = request.SeatType
        };

        var createdSeat = await _seatRepository.CreateAsync(seat);

        return MapToResponse(createdSeat);
    }

    // Create multiple seats
    public async Task<List<SeatResponse>> CreateBulkAsync(
        BulkCreateSeatsRequest request)
    {
        var screen = await _screenRepository.GetByIdAsync(
            request.ScreenId);

        if (screen == null)
            throw new InvalidOperationException("Screen not found.");

        // Check duplicates inside the request
        var duplicateRequests = request.Seats
            .GroupBy(seat => new
            {
                RowName = seat.RowName.Trim().ToUpperInvariant(),
                seat.SeatNumber
            })
            .Any(group => group.Count() > 1);

        if (duplicateRequests)
            throw new InvalidOperationException(
                "Duplicate seat positions found in the request.");

        // Check whether any requested seat already exists
        var existingSeats = await _seatRepository.GetByScreenIdAsync(
            request.ScreenId);

        foreach (var requestedSeat in request.Seats)
        {
            var exists = existingSeats.Any(existing =>
                existing.RowName.Equals(
                    requestedSeat.RowName,
                    StringComparison.OrdinalIgnoreCase) &&
                existing.SeatNumber == requestedSeat.SeatNumber);

            if (exists)
                throw new InvalidOperationException(
                    $"Seat {requestedSeat.RowName}{requestedSeat.SeatNumber} already exists.");
        }

        var seats = request.Seats.Select(requestedSeat => new Seat
        {
            ScreenId = request.ScreenId,
            RowName = requestedSeat.RowName,
            SeatNumber = requestedSeat.SeatNumber,
            SeatType = requestedSeat.SeatType
        }).ToList();

        var createdSeats = await _seatRepository.CreateBulkAsync(seats);

        return createdSeats.Select(MapToResponse).ToList();
    }

    // Get seats belonging to a screen
    public async Task<List<SeatResponse>> GetByScreenIdAsync(int screenId)
    {
        var seats = await _seatRepository.GetByScreenIdAsync(screenId);

        return seats.Select(MapToResponse).ToList();
    }

    // Get seat by ID
    public async Task<SeatResponse?> GetByIdAsync(int seatId)
    {
        var seat = await _seatRepository.GetByIdAsync(seatId);

        if (seat == null)
            return null;

        return MapToResponse(seat);
    }

    // Update seat
    public async Task<SeatResponse?> UpdateAsync(
        int seatId,
        UpdateSeatRequest request)
    {
        var existingSeat = await _seatRepository.GetByIdAsync(seatId);

        if (existingSeat == null)
            return null;

        var screen = await _screenRepository.GetByIdAsync(
            request.ScreenId);

        if (screen == null)
            throw new InvalidOperationException("Screen not found.");

        var seatsInScreen = await _seatRepository.GetByScreenIdAsync(
            request.ScreenId);

        var duplicate = seatsInScreen.Any(seat =>
            seat.SeatId != seatId &&
            seat.RowName.Equals(
                request.RowName,
                StringComparison.OrdinalIgnoreCase) &&
            seat.SeatNumber == request.SeatNumber);

        if (duplicate)
            throw new InvalidOperationException(
                "This seat position already exists.");

        var seatToUpdate = new Seat
        {
            SeatId = seatId,
            ScreenId = request.ScreenId,
            RowName = request.RowName,
            SeatNumber = request.SeatNumber,
            SeatType = request.SeatType
        };

        var updatedSeat = await _seatRepository.UpdateAsync(seatToUpdate);

        if (updatedSeat == null)
            return null;

        return MapToResponse(updatedSeat);
    }

    // Delete seat
    public async Task<bool> DeleteAsync(int seatId)
    {
        return await _seatRepository.DeleteAsync(seatId);
    }

    // Convert Seat entity into SeatResponse DTO
    private static SeatResponse MapToResponse(Seat seat)
    {
        return new SeatResponse
        {
            SeatId = seat.SeatId,
            ScreenId = seat.ScreenId,
            RowName = seat.RowName,
            SeatNumber = seat.SeatNumber,
            SeatType = seat.SeatType
        };
    }
}