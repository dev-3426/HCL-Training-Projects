namespace Enrollora.Api.Models;

public sealed class Enrollment
{
    public int EnrollmentId { get; set; }

    public int StudentId { get; set; }

    public Student Student { get; set; } = null!;

    public int CourseId { get; set; }

    public Course Course { get; set; } = null!;

    public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;

    public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Active;

    public ICollection<CancellationRequest> CancellationRequests { get; set; } =
        new List<CancellationRequest>();
}
