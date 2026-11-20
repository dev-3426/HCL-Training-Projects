using System.ComponentModel.DataAnnotations;

namespace Enrollora.Api.DTOs;

public sealed class CourseQuery : PaginationQuery
{
    public string? Search { get; set; }
    public bool? IsActive { get; set; }
    public string? Availability { get; set; }
    public decimal? MinFees { get; set; }
    public decimal? MaxFees { get; set; }
    public int? MinCapacity { get; set; }
    public int? MaxCapacity { get; set; }
}

public sealed class SaveCourseRequest
{
    [Required, MaxLength(150)]
    public string CourseName { get; set; } = string.Empty;

    [Required, MaxLength(80)]
    public string Duration { get; set; } = string.Empty;

    [Range(typeof(decimal), "0", "999999999")]
    public decimal Fees { get; set; }

    [Range(1, 100000)]
    public int Capacity { get; set; }

    public bool IsActive { get; set; } = true;
}

public sealed record CourseResponse(
    int CourseId,
    string CourseName,
    string Duration,
    decimal Fees,
    int Capacity,
    int EnrolledCount,
    int AvailableSeats,
    bool IsActive);
