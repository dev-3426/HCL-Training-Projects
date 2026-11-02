
using System.ComponentModel.DataAnnotations;

namespace MovieTicketBooking.DTOs.Movies;

public class UpdateMovieRequest
{
    [Required]
    [StringLength(150, MinimumLength = 1)]
    public string Title { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [Range(1, 1000)]
    public int DurationMinutes { get; set; }

    [Required]
    public DateTime ReleaseDate { get; set; }

    [Required]
    [StringLength(50, MinimumLength = 1)]
    public string Language { get; set; } = string.Empty;

    [Required]
    [StringLength(50, MinimumLength = 1)]
    public string Genre { get; set; } = string.Empty;
}
