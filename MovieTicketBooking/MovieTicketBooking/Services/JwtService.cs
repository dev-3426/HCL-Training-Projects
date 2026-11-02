
using Microsoft.IdentityModel.Tokens;
using MovieTicketBooking.Models;
using MovieTicketBooking.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MovieTicketBooking.Services;

public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;

    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(User user, out DateTime expiresAt)
    {
        var jwtSettings = _configuration.GetSection("Jwt");

        string issuer = jwtSettings["Issuer"]
            ?? throw new InvalidOperationException("JWT Issuer is missing.");

        string audience = jwtSettings["Audience"]
            ?? throw new InvalidOperationException("JWT Audience is missing.");

        string key = jwtSettings["Key"]
            ?? throw new InvalidOperationException("JWT Key is missing.");

        if (Encoding.UTF8.GetByteCount(key) < 32)
        {
            throw new InvalidOperationException(
                "JWT Key must contain at least 32 bytes.");
        }

        int expiryMinutes = int.TryParse(
            jwtSettings["ExpiryMinutes"], out int minutes)
            ? minutes
            : 60;

        expiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(key));

        var credentials = new SigningCredentials(
            securityKey,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
