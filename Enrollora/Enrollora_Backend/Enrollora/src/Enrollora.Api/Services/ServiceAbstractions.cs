using Enrollora.Api.DTOs;
using Enrollora.Api.Models;

namespace Enrollora.Api.Services;

public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string hash, string password);
}

public interface ITokenService
{
    LoginResponse CreateToken(string userId, string name, string email, string role);
}

public interface IAuthService
{
    Task RegisterAsync(RegisterStudentRequest request, CancellationToken ct);
    Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken ct);
}

public interface IStudentService
{
    Task<PagedResult<StudentResponse>> GetStudentsAsync(StudentQuery query, CancellationToken ct);
    Task<StudentResponse> GetByIdAsync(int id, CancellationToken ct);
    Task<StudentResponse> GetMeAsync(int studentId, CancellationToken ct);
    Task<StudentResponse> UpdateMeAsync(int studentId, UpdateStudentRequest request, CancellationToken ct);
    Task<StudentResponse> UpdateAsync(int id, UpdateStudentRequest request, CancellationToken ct);
    Task ReviewAsync(int id, StudentStatus status, CancellationToken ct);
    Task DeleteAsync(int id, CancellationToken ct);
}

public interface ICourseService
{
    Task<PagedResult<CourseResponse>> GetCoursesAsync(CourseQuery query, CancellationToken ct);
    Task<CourseResponse> GetByIdAsync(int id, CancellationToken ct);
    Task<CourseResponse> CreateAsync(SaveCourseRequest request, CancellationToken ct);
    Task<CourseResponse> UpdateAsync(int id, SaveCourseRequest request, CancellationToken ct);
    Task DeactivateAsync(int id, CancellationToken ct);
}

public interface IEnrollmentService
{
    Task<PagedResult<EnrollmentResponse>> GetAllAsync(EnrollmentQuery query, CancellationToken ct);
    Task<PagedResult<EnrollmentResponse>> GetMineAsync(int studentId, EnrollmentQuery query, CancellationToken ct);
    Task<PagedResult<EnrollmentResponse>> GetForStudentAsync(int studentId, EnrollmentQuery query, CancellationToken ct);
    Task<PagedResult<EnrollmentResponse>> GetForCourseAsync(int courseId, EnrollmentQuery query, CancellationToken ct);
    Task<EnrollmentResponse> CreateAsync(int studentId, int courseId, CancellationToken ct);
}

public interface ICancellationRequestService
{
    Task<PagedResult<CancellationResponse>> GetAllAsync(CancellationQuery query, CancellationToken ct);
    Task<PagedResult<CancellationResponse>> GetMineAsync(int studentId, CancellationQuery query, CancellationToken ct);
    Task<CancellationResponse> CreateAsync(int studentId, int enrollmentId, CreateCancellationRequest request, CancellationToken ct);
    Task<CancellationResponse> ReviewAsync(int requestId, bool approve, ReviewCancellationRequest request, CancellationToken ct);
}

public interface IDashboardService
{
    Task<DashboardSummary> GetSummaryAsync(CancellationToken ct);
}
