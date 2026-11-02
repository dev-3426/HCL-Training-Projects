
using MovieTicketBooking.DTOs.Seats;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services.Interfaces;
using MovieTicketBooking.Exceptions;

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
            throw new NotFoundException("Screen not found.");

        var exists = await _seatRepository.ExistsAsync(
            request.ScreenId,
            request.RowName,
            request.SeatNumber);

        if (exists)
            throw new ConflictException(
                "This seat position already exists.");

        var seat = new Seat
        {
            ScreenId = request.ScreenId,
            RowName = request.RowName.Trim(),
            SeatNumber = request.SeatNumber,
            SeatType = request.SeatType.Trim()
        };

        var createdSeat = await _seatRepository.CreateAsync(seat);

        return MapToResponse(createdSeat);
    }

    // Create multiple seats
    public async Task<List<SeatResponse>> CreateBulkAsync(
        BulkCreateSeatsRequest request)
    {
        if (request.Seats == null || request.Seats.Count == 0)
            throw new ValidationException(
                "At least one seat must be provided.");

        var screen = await _screenRepository.GetByIdAsync(
            request.ScreenId);

        if (screen == null)
            throw new NotFoundException("Screen not found.");

        // Check duplicates inside the request
        var duplicateRequests = request.Seats
            .GroupBy(seat => new
            {
                RowName = seat.RowName.Trim().ToUpperInvariant(),
                seat.SeatNumber
            })
            .Any(group => group.Count() > 1);

        if (duplicateRequests)
            throw new ConflictException(
                "Duplicate seat positions found in the request.");

        // Check whether any requested seat already exists
        var existingSeats = await _seatRepository.GetByScreenIdAsync(
            request.ScreenId);

        foreach (var requestedSeat in request.Seats)
        {
            var exists = existingSeats.Any(existing =>
                existing.RowName.Equals(
                    requestedSeat.RowName.Trim(),
                    StringComparison.OrdinalIgnoreCase) &&
                existing.SeatNumber == requestedSeat.SeatNumber);

            if (exists)
                throw new ConflictException(
                    $"Seat {requestedSeat.RowName}{requestedSeat.SeatNumber} already exists.");
        }

        var seats = request.Seats.Select(requestedSeat => new Seat
        {
            ScreenId = request.ScreenId,
            RowName = requestedSeat.RowName.Trim(),
            SeatNumber = requestedSeat.SeatNumber,
            SeatType = requestedSeat.SeatType.Trim()
        }).ToList();

        var createdSeats = await _seatRepository.CreateBulkAsync(seats);

        return createdSeats.Select(MapToResponse).ToList();
    }

    // Get seats belonging to a screen
    public async Task<List<SeatResponse>> GetByScreenIdAsync(int screenId)
    {
        var screen = await _screenRepository.GetByIdAsync(screenId);

        if (screen == null)
            throw new NotFoundException("Screen not found.");

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
            throw new NotFoundException(
                "Seat not found. Cannot update.");

        var screen = await _screenRepository.GetByIdAsync(
            request.ScreenId);

        if (screen == null)
            throw new NotFoundException("Screen not found.");

        var seatsInScreen = await _seatRepository.GetByScreenIdAsync(
            request.ScreenId);

        var duplicate = seatsInScreen.Any(seat =>
            seat.SeatId != seatId &&
            seat.RowName.Equals(
                request.RowName.Trim(),
                StringComparison.OrdinalIgnoreCase) &&
            seat.SeatNumber == request.SeatNumber);

        if (duplicate)
            throw new ConflictException(
                "This seat position already exists.");

        var seatToUpdate = new Seat
        {
            SeatId = seatId,
            ScreenId = request.ScreenId,
            RowName = request.RowName.Trim(),
            SeatNumber = request.SeatNumber,
            SeatType = request.SeatType.Trim()
        };

        var updatedSeat = await _seatRepository.UpdateAsync(seatToUpdate);

        if (updatedSeat == null)
            throw new NotFoundException(
                "Seat not found. Cannot update.");

        return MapToResponse(updatedSeat);
    }

    // Delete seat
    public async Task<bool> DeleteAsync(int seatId)
    {
        var seat = await _seatRepository.GetByIdAsync(seatId);

        if (seat == null)
            throw new NotFoundException(
                "Seat not found. Cannot delete.");

        bool isDeleted = await _seatRepository.DeleteAsync(seatId);

        if (!isDeleted)
            throw new ConflictException(
                "Seat could not be deleted. It may have related shows or bookings.");

        return true;
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
