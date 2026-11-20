using System.ComponentModel.DataAnnotations;

namespace Enrollora.Api.Models;

public sealed class AdminUser
{
    public int AdminUserId { get; set; }

    [MaxLength(120)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(256)]
    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
}
