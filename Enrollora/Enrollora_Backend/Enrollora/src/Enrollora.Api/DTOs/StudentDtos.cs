using System.ComponentModel.DataAnnotations;
using Enrollora.Api.Models;

namespace Enrollora.Api.DTOs;

public sealed class StudentQuery : PaginationQuery
{
    public string? Search { get; set; }
    public StudentStatus? Status { get; set; }
    public string? Department { get; set; }
    public int? MinAge { get; set; }
    public int? MaxAge { get; set; }
    public decimal? MinCgpa { get; set; }
    public decimal? MaxCgpa { get; set; }
}

public sealed class UpdateStudentRequest
{
    [Required, MaxLength(120)]
    public string Name { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(256)]
    public string Email { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string Department { get; set; } = string.Empty;

    [Range(17, 60)]
    public int Age { get; set; }

    [Range(typeof(decimal), "0", "10")]
    public decimal CGPA { get; set; }
}

public sealed record StudentResponse(
    int StudentId,
    string Name,
    string Email,
    string Department,
    int Age,
    decimal CGPA,
    StudentStatus Status,
    DateTime RegisteredAt,
    DateTime? ReviewedAt);
