using System.ComponentModel.DataAnnotations;

namespace Enrollora.Api.Models;

public sealed class Student
{
    public int StudentId { get; set; }

    [MaxLength(120)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(256)]
    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Department { get; set; } = string.Empty;

    public int Age { get; set; }

    public decimal CGPA { get; set; }

    public StudentStatus Status { get; set; } = StudentStatus.Pending;

    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;

    public DateTime? ReviewedAt { get; set; }

    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}
