using Enrollora.Api.Data;
using Enrollora.Api.DTOs;
using Enrollora.Api.Models;
using Enrollora.Api.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Enrollora.Api.Services;

public sealed class EnrollmentService(
    ApplicationDbContext db,
    IRepository<Enrollment> enrollments,
    IRepository<Student> students,
    IRepository<Course> courses) : IEnrollmentService
{
    public Task<PagedResult<EnrollmentResponse>> GetAllAsync(EnrollmentQuery query, CancellationToken ct) =>
        GetPageAsync(enrollments.Query().Include(x => x.Student).Include(x => x.Course), query, ct);

    public Task<PagedResult<EnrollmentResponse>> GetMineAsync(int studentId, EnrollmentQuery query, CancellationToken ct) =>
        GetPageAsync(enrollments.Query().Include(x => x.Student).Include(x => x.Course)
            .Where(x => x.StudentId == studentId), query, ct);

    public Task<PagedResult<EnrollmentResponse>> GetForStudentAsync(int studentId, EnrollmentQuery query, CancellationToken ct) =>
        GetPageAsync(enrollments.Query().Include(x => x.Student).Include(x => x.Course)
            .Where(x => x.StudentId == studentId), query, ct);

    public Task<PagedResult<EnrollmentResponse>> GetForCourseAsync(int courseId, EnrollmentQuery query, CancellationToken ct) =>
        GetPageAsync(enrollments.Query().Include(x => x.Student).Include(x => x.Course)
            .Where(x => x.CourseId == courseId), query, ct);

    public async Task<EnrollmentResponse> CreateAsync(int studentId, int courseId, CancellationToken ct)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(
            System.Data.IsolationLevel.Serializable, ct);

        var student = await students.Query().FirstOrDefaultAsync(x => x.StudentId == studentId, ct)
            ?? throw new KeyNotFoundException("Student was not found.");

        if (student.Status != StudentStatus.Approved)
            throw new InvalidOperationException("Only approved students can enroll.");

        var course = await courses.Query(true).FirstOrDefaultAsync(x => x.CourseId == courseId, ct)
            ?? throw new KeyNotFoundException("Course was not found.");

        if (!course.IsActive)
            throw new InvalidOperationException("This course is not available for enrollment.");

        if (await enrollments.Query().AnyAsync(
            x => x.StudentId == studentId && x.CourseId == courseId &&
                 x.Status == EnrollmentStatus.Active, ct))
            throw new InvalidOperationException("You already have an active enrollment in this course.");

        var activeCount = await enrollments.Query()
            .CountAsync(x => x.CourseId == courseId && x.Status == EnrollmentStatus.Active, ct);

        if (activeCount >= course.Capacity)
            throw new InvalidOperationException("This course has reached its enrollment capacity.");

        var enrollment = new Enrollment
        {
            StudentId = studentId,
            CourseId = courseId,
            EnrolledAt = DateTime.UtcNow,
            Status = EnrollmentStatus.Active
        };

        await enrollments.AddAsync(enrollment, ct);
        await enrollments.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);

        return new EnrollmentResponse(enrollment.EnrollmentId, studentId, student.Name,
            student.Email, courseId, course.CourseName, enrollment.EnrolledAt, enrollment.Status);
    }

    private async Task<PagedResult<EnrollmentResponse>> GetPageAsync(
        IQueryable<Enrollment> q, EnrollmentQuery query, CancellationToken ct)
    {
        query.Normalize();
        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var term = query.Search.Trim();
            q = q.Where(x => x.Student.Name.Contains(term) || x.Student.Email.Contains(term));
        }
        if (query.CourseId.HasValue) q = q.Where(x => x.CourseId == query.CourseId);
        if (query.Status.HasValue) q = q.Where(x => x.Status == query.Status);
        if (query.FromDate.HasValue) q = q.Where(x => x.EnrolledAt >= query.FromDate);
        if (query.ToDate.HasValue) q = q.Where(x => x.EnrolledAt <= query.ToDate);

        q = query.SortBy?.ToLowerInvariant() switch
        {
            "student" => query.SortOrder == "desc"
                ? q.OrderByDescending(x => x.Student.Name) : q.OrderBy(x => x.Student.Name),
            "course" => query.SortOrder == "desc"
                ? q.OrderByDescending(x => x.Course.CourseName) : q.OrderBy(x => x.Course.CourseName),
            _ => query.SortOrder == "desc"
                ? q.OrderByDescending(x => x.EnrolledAt) : q.OrderBy(x => x.EnrolledAt)
        };

        var total = await q.CountAsync(ct);
        var items = await q.Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize).Select(x => new EnrollmentResponse(
                x.EnrollmentId, x.StudentId, x.Student.Name, x.Student.Email,
                x.CourseId, x.Course.CourseName, x.EnrolledAt, x.Status)).ToListAsync(ct);

        return new PagedResult<EnrollmentResponse>(items, query.PageNumber, query.PageSize,
            total, (int)Math.Ceiling(total / (double)query.PageSize));
    }
}
