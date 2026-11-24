using System.Security.Claims;
using Enrollora.Api.DTOs;
using Enrollora.Api.Models;
using Enrollora.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Enrollora.Api.Controllers;

[ApiController]
[Route("api/students")]
[Authorize]
public sealed class StudentsController(IStudentService service) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = "Administrator")]
    public Task<PagedResult<StudentResponse>> GetAll(
        [FromQuery] StudentQuery query, CancellationToken ct) =>
        service.GetStudentsAsync(query, ct);

    [HttpGet("{id:int}")]
    [Authorize(Roles = "Administrator")]
    public Task<StudentResponse> GetById(int id, CancellationToken ct) =>
        service.GetByIdAsync(id, ct);

    [HttpGet("me")]
    [Authorize(Roles = "Student")]
    public Task<StudentResponse> GetMe(CancellationToken ct) =>
        service.GetMeAsync(CurrentStudentId(), ct);

    [HttpPut("me")]
    [Authorize(Roles = "Student")]
    public Task<StudentResponse> UpdateMe(
        UpdateStudentRequest request, CancellationToken ct) =>
        service.UpdateMeAsync(CurrentStudentId(), request, ct);

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Administrator")]
    public Task<StudentResponse> Update(
        int id, UpdateStudentRequest request, CancellationToken ct) =>
        service.UpdateAsync(id, request, ct);

    [HttpPut("{id:int}/approve")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Approve(int id, CancellationToken ct)
    {
        await service.ReviewAsync(id, StudentStatus.Approved, ct);
        return Ok(new ApiMessage("Student approved."));
    }

    [HttpPut("{id:int}/reject")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Reject(int id, CancellationToken ct)
    {
        await service.ReviewAsync(id, StudentStatus.Rejected, ct);
        return Ok(new ApiMessage("Student registration rejected."));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await service.DeleteAsync(id, ct);
        return Ok(new ApiMessage("Student deleted."));
    }

    private int CurrentStudentId() =>
        int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var studentId)
            ? studentId
            : throw new UnauthorizedAccessException("Student identity is missing or invalid.");
}
