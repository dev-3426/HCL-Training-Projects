
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Constants;
using MovieTicketBooking.Models;

namespace MovieTicketBooking.Data;

public static class RoleSeeder
{
    public static async Task SeedAsync(
        IServiceProvider serviceProvider,
        IConfiguration configuration)
    {
        using var scope = serviceProvider.CreateScope();

        var context = scope.ServiceProvider
            .GetRequiredService<ApplicationDbContext>();

        var passwordHasher = scope.ServiceProvider
            .GetRequiredService<IPasswordHasher<User>>();

        var adminSettings = configuration.GetSection("AdminSeed");

        string? fullName = adminSettings["FullName"];
        string? email = adminSettings["Email"];
        string? password = adminSettings["Password"];

        // Do not seed an account unless all credentials are configured.
        if (string.IsNullOrWhiteSpace(fullName) ||
            string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(password))
        {
            return;
        }

        email = email.Trim().ToLowerInvariant();

        var existingAdmin = await context.Users
            .FirstOrDefaultAsync(user => user.Email == email);

        if (existingAdmin != null)
        {
            return;
        }

        var admin = new User
        {
            FullName = fullName.Trim(),
            Email = email,
            Role = UserRoles.Admin,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        admin.PasswordHash = passwordHasher.HashPassword(
            admin,
            password);

        await context.Users.AddAsync(admin);

        await context.SaveChangesAsync();
    }
}
