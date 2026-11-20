using System.ComponentModel.DataAnnotations;

namespace Enrollora.Api.Models;

public sealed class Course
{
    public int CourseId { get; set; }

    [MaxLength(150)]
    public string CourseName { get; set; } = string.Empty;

    [MaxLength(80)]
    public string Duration { get; set; } = string.Empty;

    public decimal Fees { get; set; }

    public int Capacity { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}
