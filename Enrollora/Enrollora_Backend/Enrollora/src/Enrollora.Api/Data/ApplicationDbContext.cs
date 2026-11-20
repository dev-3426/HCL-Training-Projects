using Enrollora.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Enrollora.Api.Data;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options)
{
    public DbSet<Student> Students => Set<Student>();
    public DbSet<AdminUser> AdminUsers => Set<AdminUser>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();
    public DbSet<CancellationRequest> CancellationRequests => Set<CancellationRequest>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Student>()
            .HasIndex(x => x.Email)
            .IsUnique();

        modelBuilder.Entity<AdminUser>()
            .HasIndex(x => x.Email)
            .IsUnique();

        modelBuilder.Entity<Course>()
            .Property(x => x.Fees)
            .HasPrecision(12, 2);

        modelBuilder.Entity<Student>()
            .Property(x => x.CGPA)
            .HasPrecision(4, 2);

        modelBuilder.Entity<Student>()
            .ToTable(t => t.HasCheckConstraint("CK_Students_Age", "[Age] >= 17 AND [Age] <= 60"));

        modelBuilder.Entity<Student>()
            .ToTable(t => t.HasCheckConstraint("CK_Students_CGPA", "[CGPA] >= 0 AND [CGPA] <= 10"));

        modelBuilder.Entity<Course>()
            .ToTable(t => t.HasCheckConstraint("CK_Courses_Capacity", "[Capacity] > 0"));

        modelBuilder.Entity<Course>()
            .ToTable(t => t.HasCheckConstraint("CK_Courses_Fees", "[Fees] >= 0"));

        modelBuilder.Entity<Enrollment>()
            .HasOne(x => x.Student)
            .WithMany(x => x.Enrollments)
            .HasForeignKey(x => x.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Enrollment>()
            .HasOne(x => x.Course)
            .WithMany(x => x.Enrollments)
            .HasForeignKey(x => x.CourseId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CancellationRequest>()
            .HasOne(x => x.Student)
            .WithMany()
            .HasForeignKey(x => x.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CancellationRequest>()
            .HasOne(x => x.Enrollment)
            .WithMany(x => x.CancellationRequests)
            .HasForeignKey(x => x.EnrollmentId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Enrollment>()
            .HasIndex(x => new { x.StudentId, x.CourseId, x.Status });

        modelBuilder.Entity<CancellationRequest>()
            .HasIndex(x => new { x.EnrollmentId, x.Status });
    }
}
