using Enrollora.Api.DTOs;
using Enrollora.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Enrollora.Api.Controllers;

[ApiController]
[Route("api/dashboard")]
[Authorize(Roles = "Administrator")]
public sealed class DashboardController(IDashboardService service) : ControllerBase
{
    [HttpGet("summary")]
    public Task<DashboardSummary> GetSummary(CancellationToken ct) =>
        service.GetSummaryAsync(ct);
}
