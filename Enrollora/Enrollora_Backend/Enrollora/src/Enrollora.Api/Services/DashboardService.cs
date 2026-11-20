using Enrollora.Api.Data;
using Enrollora.Api.DTOs;
using Enrollora.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Enrollora.Api.Services;

public sealed class DashboardService(ApplicationDbContext db) : IDashboardService
{
    public async Task<DashboardSummary> GetSummaryAsync(CancellationToken ct)
    {
        var totalStudents = await db.Students.CountAsync(ct);
        var pending = await db.Students.CountAsync(x => x.Status == StudentStatus.Pending, ct);
        var approved = await db.Students.CountAsync(x => x.Status == StudentStatus.Approved, ct);
        var rejected = await db.Students.CountAsync(x => x.Status == StudentStatus.Rejected, ct);
        var totalCourses = await db.Courses.CountAsync(ct);
        var activeCourses = await db.Courses.CountAsync(x => x.IsActive, ct);
        var activeEnrollments = await db.Enrollments.CountAsync(x => x.Status == EnrollmentStatus.Active, ct);
        var cancelledEnrollments = await db.Enrollments.CountAsync(x => x.Status == EnrollmentStatus.Cancelled, ct);
        var pendingRequests = await db.CancellationRequests.CountAsync(
            x => x.Status == CancellationStatus.Pending, ct);
        var averageCgpa = totalStudents == 0 ? 0 :
            await db.Students.AverageAsync(x => (decimal?)x.CGPA, ct) ?? 0;

        var activeCounts = await db.Enrollments
            .Where(x => x.Status == EnrollmentStatus.Active)
            .GroupBy(x => x.CourseId)
            .Select(g => new { CourseId = g.Key, Count = g.Count() })
            .ToListAsync(ct);

        var courses = await db.Courses.Where(x => x.IsActive)
            .Select(x => new { x.CourseId, x.Capacity }).ToListAsync(ct);

        var available = courses.Sum(c =>
            Math.Max(0, c.Capacity - (activeCounts.FirstOrDefault(a => a.CourseId == c.CourseId)?.Count ?? 0)));

        return new DashboardSummary(totalStudents, pending, approved, rejected,
            totalCourses, activeCourses, activeEnrollments, cancelledEnrollments,
            pendingRequests, Math.Round(averageCgpa, 2), available);
    }
}
