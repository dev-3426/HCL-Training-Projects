
using EmployeeHub.API.Models;

namespace EmployeeHub.API.Services.Interfaces
{
    public interface IEmployeeService
    {
        Task<List<Employee>> GetAllEmployeesAsync();

        Task<Employee?> GetEmployeeByIdAsync(int id);

        Task<Employee> CreateEmployeeAsync(Employee employee);

        Task<bool> UpdateEmployeeAsync(int id, Employee employee);

        Task<bool> DeleteEmployeeAsync(int id);

        Task<List<Employee>> SearchEmployeesAsync(
            string? name, bool? isActive);

        Task<object> GetEmployeesPagedAsync(
            int pageNumber, int pageSize);
    }
}
