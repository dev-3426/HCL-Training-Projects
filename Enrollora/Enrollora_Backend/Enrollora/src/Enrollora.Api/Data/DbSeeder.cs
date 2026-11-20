using Enrollora.Api.Models;
using Enrollora.Api.Services;
using Microsoft.EntityFrameworkCore;

namespace Enrollora.Api.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider services, IConfiguration configuration)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await db.Database.MigrateAsync();

        if (await db.AdminUsers.AnyAsync())
            return;

        var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var seed = configuration.GetSection("SeedAdmin");
        var email = seed["Email"] ?? "admin@enrollora.local";
        var password = seed["Password"] ?? "Admin@123";

        db.AdminUsers.Add(new AdminUser
        {
            Name = seed["Name"] ?? "System Administrator",
            Email = email.Trim().ToLowerInvariant(),
            PasswordHash = hasher.Hash(password)
        });

        await db.SaveChangesAsync();
    }
}
