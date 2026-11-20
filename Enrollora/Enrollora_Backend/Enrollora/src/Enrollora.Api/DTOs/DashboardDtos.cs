namespace Enrollora.Api.DTOs;

public sealed record DashboardSummary(
    int TotalStudents,
    int PendingApprovals,
    int ApprovedStudents,
    int RejectedStudents,
    int TotalCourses,
    int ActiveCourses,
    int ActiveEnrollments,
    int CancelledEnrollments,
    int PendingCancellationRequests,
    decimal AverageCgpa,
    int TotalAvailableSeats);
