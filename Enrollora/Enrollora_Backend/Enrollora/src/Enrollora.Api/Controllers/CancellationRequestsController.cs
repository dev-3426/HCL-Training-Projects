using System.Security.Claims;
using Enrollora.Api.DTOs;
using Enrollora.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Enrollora.Api.Controllers;

[ApiController]
[Route("api/cancellation-requests")]
[Authorize]
public sealed class CancellationRequestsController(
    ICancellationRequestService service) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = "Administrator")]
    public Task<PagedResult<CancellationResponse>> GetAll(
        [FromQuery] CancellationQuery query, CancellationToken ct) =>
        service.GetAllAsync(query, ct);

    [HttpGet("my")]
    [Authorize(Roles = "Student")]
    public Task<PagedResult<CancellationResponse>> GetMine(
        [FromQuery] CancellationQuery query, CancellationToken ct) =>
        service.GetMineAsync(CurrentStudentId(), query, ct);

    [HttpPost("~/api/enrollments/{enrollmentId:int}/cancellation-request")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> Create(
        int enrollmentId, CreateCancellationRequest request, CancellationToken ct)
    {
        var result = await service.CreateAsync(CurrentStudentId(), enrollmentId, request, ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }

    [HttpPut("{id:int}/approve")]
    [Authorize(Roles = "Administrator")]
    public Task<CancellationResponse> Approve(
        int id, ReviewCancellationRequest request, CancellationToken ct) =>
        service.ReviewAsync(id, true, request, ct);

    [HttpPut("{id:int}/reject")]
    [Authorize(Roles = "Administrator")]
    public Task<CancellationResponse> Reject(
        int id, ReviewCancellationRequest request, CancellationToken ct) =>
        service.ReviewAsync(id, false, request, ct);

    private int CurrentStudentId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException("Student identity is missing."));
}
