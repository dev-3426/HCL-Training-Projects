
using System.ComponentModel.DataAnnotations;

namespace MovieTicketBooking.DTOs.Seats;

public class BulkCreateSeatsRequest
{
    [Range(1, int.MaxValue,
        ErrorMessage = "Please select a valid screen.")]
    public int ScreenId { get; set; }

    [Required]
    [MinLength(1, ErrorMessage = "At least one seat is required.")]
    public List<BulkSeatItemRequest> Seats { get; set; } = new();
}

public class BulkSeatItemRequest
{
    [Required]
    [StringLength(5, MinimumLength = 1)]
    public string RowName { get; set; } = string.Empty;

    [Range(1, int.MaxValue,
        ErrorMessage = "Seat number must be greater than zero.")]
    public int SeatNumber { get; set; }

    [Required]
    [StringLength(30, MinimumLength = 1)]
    public string SeatType { get; set; } = string.Empty;
}
