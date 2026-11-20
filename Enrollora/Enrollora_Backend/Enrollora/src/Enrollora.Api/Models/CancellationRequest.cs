using System.ComponentModel.DataAnnotations;

namespace Enrollora.Api.Models;

public sealed class CancellationRequest
{
    public int CancellationRequestId { get; set; }

    public int EnrollmentId { get; set; }

    public Enrollment Enrollment { get; set; } = null!;

    public int StudentId { get; set; }

    public Student Student { get; set; } = null!;

    [MaxLength(1000)]
    public string Reason { get; set; } = string.Empty;

    public CancellationStatus Status { get; set; } = CancellationStatus.Pending;

    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;

    public DateTime? ReviewedAt { get; set; }

    [MaxLength(1000)]
    public string? AdminRemarks { get; set; }
}
