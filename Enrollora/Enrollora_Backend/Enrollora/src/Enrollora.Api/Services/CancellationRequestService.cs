using Enrollora.Api.DTOs;
using Enrollora.Api.Models;
using Enrollora.Api.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Enrollora.Api.Services;

public sealed class CancellationRequestService(
    IRepository<CancellationRequest> requests,
    IRepository<Enrollment> enrollments) : ICancellationRequestService
{
    public Task<PagedResult<CancellationResponse>> GetAllAsync(CancellationQuery query, CancellationToken ct) =>
        GetPageAsync(requests.Query().Include(x => x.Student).Include(x => x.Enrollment)
            .ThenInclude(x => x.Course), query, ct);

    public Task<PagedResult<CancellationResponse>> GetMineAsync(
        int studentId, CancellationQuery query, CancellationToken ct) =>
        GetPageAsync(requests.Query().Include(x => x.Student).Include(x => x.Enrollment)
            .ThenInclude(x => x.Course).Where(x => x.StudentId == studentId), query, ct);

    public async Task<CancellationResponse> CreateAsync(
        int studentId, int enrollmentId, CreateCancellationRequest request, CancellationToken ct)
    {
        var enrollment = await enrollments.Query()
            .Include(x => x.Student).Include(x => x.Course)
            .FirstOrDefaultAsync(x => x.EnrollmentId == enrollmentId && x.StudentId == studentId, ct)
            ?? throw new KeyNotFoundException("Enrollment was not found.");

        if (enrollment.Status != EnrollmentStatus.Active)
            throw new InvalidOperationException("Only active enrollments can be cancelled.");

        if (await requests.Query().AnyAsync(
            x => x.EnrollmentId == enrollmentId && x.Status == CancellationStatus.Pending, ct))
            throw new InvalidOperationException("A pending cancellation request already exists.");

        var entity = new CancellationRequest
        {
            EnrollmentId = enrollmentId,
            StudentId = studentId,
            Reason = request.Reason.Trim(),
            Status = CancellationStatus.Pending,
            RequestedAt = DateTime.UtcNow
        };

        await requests.AddAsync(entity, ct);
        await requests.SaveChangesAsync(ct);

        return Map(entity, enrollment.Student, enrollment.Course);
    }

    public async Task<CancellationResponse> ReviewAsync(
        int requestId, bool approve, ReviewCancellationRequest request, CancellationToken ct)
    {
        var entity = await requests.Query(true)
            .Include(x => x.Student)
            .Include(x => x.Enrollment).ThenInclude(x => x.Course)
            .FirstOrDefaultAsync(x => x.CancellationRequestId == requestId, ct)
            ?? throw new KeyNotFoundException("Cancellation request was not found.");

        if (entity.Status != CancellationStatus.Pending)
            throw new InvalidOperationException("This request has already been reviewed.");

        entity.Status = approve ? CancellationStatus.Approved : CancellationStatus.Rejected;
        entity.ReviewedAt = DateTime.UtcNow;
        entity.AdminRemarks = request.AdminRemarks?.Trim();

        if (approve)
            entity.Enrollment.Status = EnrollmentStatus.Cancelled;

        requests.Update(entity);
        await requests.SaveChangesAsync(ct);

        return Map(entity, entity.Student, entity.Enrollment.Course);
    }

    private async Task<PagedResult<CancellationResponse>> GetPageAsync(
        IQueryable<CancellationRequest> q, CancellationQuery query, CancellationToken ct)
    {
        query.Normalize();
        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var term = query.Search.Trim();
            q = q.Where(x => x.Student.Name.Contains(term) || x.Student.Email.Contains(term));
        }
        if (query.CourseId.HasValue) q = q.Where(x => x.Enrollment.CourseId == query.CourseId);
        if (query.Status.HasValue) q = q.Where(x => x.Status == query.Status);
        if (query.FromDate.HasValue) q = q.Where(x => x.RequestedAt >= query.FromDate);
        if (query.ToDate.HasValue) q = q.Where(x => x.RequestedAt <= query.ToDate);

        q = query.SortOrder == "desc"
            ? q.OrderByDescending(x => x.RequestedAt)
            : q.OrderBy(x => x.RequestedAt);

        var total = await q.CountAsync(ct);
        var items = await q.Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(x => new CancellationResponse(
                x.CancellationRequestId, x.EnrollmentId, x.StudentId,
                x.Student.Name, x.Student.Email, x.Enrollment.Course.CourseName,
                x.Reason, x.Status, x.RequestedAt, x.ReviewedAt, x.AdminRemarks))
            .ToListAsync(ct);

        return new PagedResult<CancellationResponse>(items, query.PageNumber, query.PageSize,
            total, (int)Math.Ceiling(total / (double)query.PageSize));
    }

    private static CancellationResponse Map(
        CancellationRequest x, Student student, Course course) =>
        new(x.CancellationRequestId, x.EnrollmentId, x.StudentId,
            student.Name, student.Email, course.CourseName, x.Reason,
            x.Status, x.RequestedAt, x.ReviewedAt, x.AdminRemarks);
}
