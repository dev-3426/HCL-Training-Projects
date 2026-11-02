using MovieTicketBooking.DTOs.Seats;

namespace MovieTicketBooking.Services.Interfaces;

public interface ISeatService
{
    Task<SeatResponse> CreateAsync(CreateSeatRequest request);

    Task<List<SeatResponse>> CreateBulkAsync(
        BulkCreateSeatsRequest request);

    Task<List<SeatResponse>> GetByScreenIdAsync(int screenId);

    Task<SeatResponse?> GetByIdAsync(int seatId);

    Task<SeatResponse?> UpdateAsync(
        int seatId,
        UpdateSeatRequest request);

    Task<bool> DeleteAsync(int seatId);
}