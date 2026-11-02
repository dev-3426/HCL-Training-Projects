
using System.ComponentModel.DataAnnotations;

namespace MovieTicketBooking.DTOs.Shows;

public class UpdateShowRequest : IValidatableObject
{
    [Range(1, int.MaxValue,
        ErrorMessage = "Please select a valid movie.")]
    public int MovieId { get; set; }

    [Range(1, int.MaxValue,
        ErrorMessage = "Please select a valid screen.")]
    public int ScreenId { get; set; }

    [Required]
    public DateTime StartTime { get; set; }

    [Required]
    public DateTime EndTime { get; set; }

    [Range(typeof(decimal), "0", "99999999.99",
        ErrorMessage = "Ticket price must be between 0 and 99999999.99.")]
    public decimal TicketPrice { get; set; }

    public IEnumerable<ValidationResult> Validate(
        ValidationContext validationContext)
    {
        if (StartTime == default)
        {
            yield return new ValidationResult(
                "Start time is required.",
                new[] { nameof(StartTime) });
        }

        if (EndTime == default)
        {
            yield return new ValidationResult(
                "End time is required.",
                new[] { nameof(EndTime) });
        }

        if (StartTime != default &&
            EndTime != default &&
            EndTime <= StartTime)
        {
            yield return new ValidationResult(
                "End time must be later than start time.",
                new[] { nameof(EndTime) });
        }
    }
}
