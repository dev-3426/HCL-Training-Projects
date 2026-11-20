using Enrollora.Api.DTOs;
using Enrollora.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Enrollora.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(IAuthService authService) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register(
        RegisterStudentRequest request, CancellationToken ct)
    {
        await authService.RegisterAsync(request, ct);
        return StatusCode(StatusCodes.Status201Created,
            new ApiMessage("Registration submitted. Wait for administrator approval."));
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(
        LoginRequest request, CancellationToken ct) =>
        Ok(await authService.LoginAsync(request, ct));
}
