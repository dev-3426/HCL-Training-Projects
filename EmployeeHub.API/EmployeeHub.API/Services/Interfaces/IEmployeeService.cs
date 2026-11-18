
using EmployeeHub.API.DTOs;

namespace EmployeeHub.API.Services.Interfaces
{
    public interface IEmployeeService
    {
        Task<List<EmployeeResponseDto>> GetAllEmployeesAsync();

        Task<EmployeeResponseDto?> GetEmployeeByIdAsync(int id);

        Task<EmployeeResponseDto> CreateEmployeeAsync(CreateEmployeeDto dto);

        Task<bool> UpdateEmployeeAsync(int id, UpdateEmployeeDto dto);

        Task<bool> DeleteEmployeeAsync(int id);

        Task<List<EmployeeResponseDto>> SearchEmployeesAsync(
            string? name, bool? isActive);

        Task<object> GetEmployeesPagedAsync(
            int pageNumber, int pageSize);
    }
}
