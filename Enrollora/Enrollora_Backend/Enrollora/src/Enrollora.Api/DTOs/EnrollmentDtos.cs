using Enrollora.Api.Models;

namespace Enrollora.Api.DTOs;

public sealed class EnrollmentQuery : PaginationQuery
{
    public string? Search { get; set; }
    public int? CourseId { get; set; }
    public EnrollmentStatus? Status { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}

public sealed class CreateEnrollmentRequest
{
    public int CourseId { get; set; }
}

public sealed record EnrollmentResponse(
    int EnrollmentId,
    int StudentId,
    string StudentName,
    string StudentEmail,
    int CourseId,
    string CourseName,
    DateTime EnrolledAt,
    EnrollmentStatus Status);
