using System.Security.Claims;
using Enrollora.Api.DTOs;
using Enrollora.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Enrollora.Api.Controllers;

[ApiController]
[Route("api/enrollments")]
[Authorize]
public sealed class EnrollmentsController(IEnrollmentService service) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = "Administrator")]
    public Task<PagedResult<EnrollmentResponse>> GetAll(
        [FromQuery] EnrollmentQuery query, CancellationToken ct) =>
        service.GetAllAsync(query, ct);

    [HttpGet("my")]
    [Authorize(Roles = "Student")]
    public Task<PagedResult<EnrollmentResponse>> GetMine(
        [FromQuery] EnrollmentQuery query, CancellationToken ct) =>
        service.GetMineAsync(CurrentStudentId(), query, ct);

    [HttpGet("student/{studentId:int}")]
    [Authorize(Roles = "Administrator")]
    public Task<PagedResult<EnrollmentResponse>> GetForStudent(
        int studentId, [FromQuery] EnrollmentQuery query, CancellationToken ct) =>
        service.GetForStudentAsync(studentId, query, ct);

    [HttpGet("course/{courseId:int}")]
    [Authorize(Roles = "Administrator")]
    public Task<PagedResult<EnrollmentResponse>> GetForCourse(
        int courseId, [FromQuery] EnrollmentQuery query, CancellationToken ct) =>
        service.GetForCourseAsync(courseId, query, ct);

    [HttpPost]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> Create(
        CreateEnrollmentRequest request, CancellationToken ct)
    {
        var result = await service.CreateAsync(CurrentStudentId(), request.CourseId, ct);
        return CreatedAtAction(nameof(GetMine), result);
    }

    private int CurrentStudentId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException("Student identity is missing."));
}
