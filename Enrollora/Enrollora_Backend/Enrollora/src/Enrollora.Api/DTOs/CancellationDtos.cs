using System.ComponentModel.DataAnnotations;
using Enrollora.Api.Models;

namespace Enrollora.Api.DTOs;

public sealed class CancellationQuery : PaginationQuery
{
    public string? Search { get; set; }
    public int? CourseId { get; set; }
    public CancellationStatus? Status { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}

public sealed class CreateCancellationRequest
{
    [Required, MaxLength(1000)]
    public string Reason { get; set; } = string.Empty;
}

public sealed class ReviewCancellationRequest
{
    [MaxLength(1000)]
    public string? AdminRemarks { get; set; }
}

public sealed record CancellationResponse(
    int CancellationRequestId,
    int EnrollmentId,
    int StudentId,
    string StudentName,
    string StudentEmail,
    string CourseName,
    string Reason,
    CancellationStatus Status,
    DateTime RequestedAt,
    DateTime? ReviewedAt,
    string? AdminRemarks);
