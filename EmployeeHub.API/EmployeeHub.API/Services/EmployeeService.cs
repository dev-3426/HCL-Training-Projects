
using EmployeeHub.API.Data;
using EmployeeHub.API.Models;
using EmployeeHub.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EmployeeHub.API.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly ApplicationDbContext _context;

        public EmployeeService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Get all employees
        public async Task<List<Employee>> GetAllEmployeesAsync()
        {
            return await _context.Employees.ToListAsync();
        }

        // Get employee by ID
        public async Task<Employee?> GetEmployeeByIdAsync(int id)
        {
            return await _context.Employees
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        // Create employee
        public async Task<Employee> CreateEmployeeAsync(Employee employee)
        {
            _context.Employees.Add(employee);

            await _context.SaveChangesAsync();

            return employee;
        }

        // Update employee
        public async Task<bool> UpdateEmployeeAsync(int id, Employee employee)
        {
            var existingEmployee = await _context.Employees
                .FirstOrDefaultAsync(e => e.Id == id);

            if (existingEmployee == null)
            {
                return false;
            }

            existingEmployee.FirstName = employee.FirstName;
            existingEmployee.LastName = employee.LastName;
            existingEmployee.Email = employee.Email;
            existingEmployee.PhoneNumber = employee.PhoneNumber;
            existingEmployee.Salary = employee.Salary;
            existingEmployee.DateOfJoining = employee.DateOfJoining;
            existingEmployee.IsActive = employee.IsActive;

            await _context.SaveChangesAsync();

            return true;
        }

        // Delete employee
        public async Task<bool> DeleteEmployeeAsync(int id)
        {
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.Id == id);

            if (employee == null)
            {
                return false;
            }

            _context.Employees.Remove(employee);

            await _context.SaveChangesAsync();

            return true;
        }



        // Search and filter employees
        public async Task<List<Employee>> SearchEmployeesAsync(
            string? name,
            bool? isActive)
        {
            var query = _context.Employees.AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(e =>
                    e.FirstName.Contains(name) ||
                    e.LastName.Contains(name));
            }

            if (isActive.HasValue)
            {
                query = query.Where(e => e.IsActive == isActive.Value);
            }

            return await query.ToListAsync();
        }

        // Get employees with pagination
        public async Task<object> GetEmployeesPagedAsync(
            int pageNumber,
            int pageSize)
        {
            var totalRecords = await _context.Employees.CountAsync();

            var employees = await _context.Employees
                .OrderBy(e => e.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var totalPages = (int)Math.Ceiling(
                totalRecords / (double)pageSize);

            return new
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalRecords,
                TotalPages = totalPages,
                Employees = employees
            };
        }

    }
}
