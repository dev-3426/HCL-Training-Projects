using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Enrollora.Api.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Enrollora.Api.Services;

public sealed class PasswordHasher : IPasswordHasher
{
    private readonly PasswordHasher<object> _hasher = new();

    public string Hash(string password) =>
        _hasher.HashPassword(new object(), password);

    public bool Verify(string hash, string password) =>
        _hasher.VerifyHashedPassword(new object(), hash, password)
        != PasswordVerificationResult.Failed;
}

public sealed class TokenService(IConfiguration configuration) : ITokenService
{
    public LoginResponse CreateToken(string userId, string name, string email, string role)
    {
        var expiry = DateTime.UtcNow.AddMinutes(
            int.TryParse(configuration["Jwt:ExpiryMinutes"], out var minutes) ? minutes : 120);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Name, name),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, role)
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            configuration["Jwt:Issuer"],
            configuration["Jwt:Audience"],
            claims,
            expires: expiry,
            signingCredentials: credentials);

        return new LoginResponse(
            new JwtSecurityTokenHandler().WriteToken(token),
            role,
            name,
            email,
            expiry);
    }
}
