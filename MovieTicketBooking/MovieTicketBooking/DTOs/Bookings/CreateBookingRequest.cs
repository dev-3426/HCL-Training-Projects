
using System.ComponentModel.DataAnnotations;

namespace MovieTicketBooking.DTOs.Bookings;

public class CreateBookingRequest
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string CustomerName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(150)]
    public string CustomerEmail { get; set; } = string.Empty;

    [Required]
    [RegularExpression(
        @"^[0-9]{10,15}$",
        ErrorMessage = "Phone number must contain 10 to 15 digits.")]
    public string CustomerPhone { get; set; } = string.Empty;

    [Range(1, int.MaxValue,
        ErrorMessage = "Please select a valid show.")]
    public int ShowId { get; set; }

    [Required]
    [MinLength(1, ErrorMessage = "Select at least one seat.")]
    public List<int> SeatIds { get; set; } = new();
}
