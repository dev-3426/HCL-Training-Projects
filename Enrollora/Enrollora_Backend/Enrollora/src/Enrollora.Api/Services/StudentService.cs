using Enrollora.Api.DTOs;
using Enrollora.Api.Models;
using Enrollora.Api.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Enrollora.Api.Services;

public sealed class StudentService(IRepository<Student> students) : IStudentService
{
    public async Task<PagedResult<StudentResponse>> GetStudentsAsync(StudentQuery query, CancellationToken ct)
    {
        query.Normalize();
        var q = students.Query();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var term = query.Search.Trim();
            q = q.Where(x => x.Name.Contains(term) || x.Email.Contains(term));
        }

        if (query.Status.HasValue) q = q.Where(x => x.Status == query.Status);
        if (!string.IsNullOrWhiteSpace(query.Department))
            q = q.Where(x => x.Department == query.Department);
        if (query.MinAge.HasValue) q = q.Where(x => x.Age >= query.MinAge);
        if (query.MaxAge.HasValue) q = q.Where(x => x.Age <= query.MaxAge);
        if (query.MinCgpa.HasValue) q = q.Where(x => x.CGPA >= query.MinCgpa);
        if (query.MaxCgpa.HasValue) q = q.Where(x => x.CGPA <= query.MaxCgpa);

        q = (query.SortBy?.ToLowerInvariant(), query.SortOrder) switch
        {
            ("name", "desc") => q.OrderByDescending(x => x.Name),
            ("age", "desc") => q.OrderByDescending(x => x.Age),
            ("age", _) => q.OrderBy(x => x.Age),
            ("cgpa", "desc") => q.OrderByDescending(x => x.CGPA),
            ("cgpa", _) => q.OrderBy(x => x.CGPA),
            ("registeredat", "desc") => q.OrderByDescending(x => x.RegisteredAt),
            ("registeredat", _) => q.OrderBy(x => x.RegisteredAt),
            ("name", _) => q.OrderBy(x => x.Name),
            _ => q.OrderByDescending(x => x.RegisteredAt)
        };

        var page = await QueryHelpers.ToPagedResultAsync(q, query, ct);
        return new PagedResult<StudentResponse>(
            page.Items.Select(Map).ToList(), page.PageNumber, page.PageSize,
            page.TotalRecords, page.TotalPages);
    }

    public async Task<StudentResponse> GetByIdAsync(int id, CancellationToken ct) =>
        Map(await FindAsync(id, ct));

    public async Task<StudentResponse> GetMeAsync(int studentId, CancellationToken ct) =>
        Map(await FindAsync(studentId, ct));

    public async Task<StudentResponse> UpdateMeAsync(
        int studentId, UpdateStudentRequest request, CancellationToken ct) =>
        await UpdateAsync(studentId, request, ct);

    public async Task<StudentResponse> UpdateAsync(
        int id, UpdateStudentRequest request, CancellationToken ct)
    {
        var student = await FindAsync(id, ct);
        student.Name = request.Name.Trim();
        student.Department = request.Department.Trim();
        student.Age = request.Age;
        student.CGPA = request.CGPA;
        students.Update(student);
        await students.SaveChangesAsync(ct);
        return Map(student);
    }

    public async Task ReviewAsync(int id, StudentStatus status, CancellationToken ct)
    {
        if (status is not (StudentStatus.Approved or StudentStatus.Rejected))
            throw new ArgumentException("Review status must be Approved or Rejected.");

        var student = await FindAsync(id, ct);
        if (student.Status != StudentStatus.Pending)
            throw new InvalidOperationException("Only pending registrations can be reviewed.");

        student.Status = status;
        student.ReviewedAt = DateTime.UtcNow;
        students.Update(student);
        await students.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct)
    {
        var student = await FindAsync(id, ct);
        if (await students.Query().Where(x => x.StudentId == id)
            .SelectMany(x => x.Enrollments).AnyAsync(ct))
            throw new InvalidOperationException("Student has enrollment history and cannot be deleted.");

        students.Remove(student);
        await students.SaveChangesAsync(ct);
    }

    private async Task<Student> FindAsync(int id, CancellationToken ct) =>
        await students.Query(true).FirstOrDefaultAsync(x => x.StudentId == id, ct)
        ?? throw new KeyNotFoundException("Student was not found.");

    private static StudentResponse Map(Student x) =>
        new(x.StudentId, x.Name, x.Email, x.Department, x.Age, x.CGPA,
            x.Status, x.RegisteredAt, x.ReviewedAt);
}
