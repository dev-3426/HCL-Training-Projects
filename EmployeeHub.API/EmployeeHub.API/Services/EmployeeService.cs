
using EmployeeHub.API.Data;
using EmployeeHub.API.DTOs;
using EmployeeHub.API.DTOs.Mappings;
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
        public async Task<List<EmployeeResponseDto>> GetAllEmployeesAsync()
        {
            var employees = await _context.Employees.ToListAsync();

            return employees
                .Select(EmployeeMapper.ToResponseDto)
                .ToList();
        }

        // Get employee by ID
        public async Task<EmployeeResponseDto?> GetEmployeeByIdAsync(int id)
        {
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.Id == id);

            if (employee == null)
            {
                return null;
            }

            return EmployeeMapper.ToResponseDto(employee);
        }

        // Create employee
        public async Task<EmployeeResponseDto> CreateEmployeeAsync(
            CreateEmployeeDto dto)
        {
            var employee = EmployeeMapper.ToEmployee(dto);

            _context.Employees.Add(employee);

            await _context.SaveChangesAsync();

            return EmployeeMapper.ToResponseDto(employee);
        }

        // Update employee
        public async Task<bool> UpdateEmployeeAsync(
            int id, UpdateEmployeeDto dto)
        {
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.Id == id);

            if (employee == null)
            {
                return false;
            }

            EmployeeMapper.UpdateEmployeeFromDto(employee, dto);

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
        public async Task<List<EmployeeResponseDto>> SearchEmployeesAsync(
            string? name, bool? isActive)
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

            var employees = await query.ToListAsync();

            return employees
                .Select(EmployeeMapper.ToResponseDto)
                .ToList();
        }

        // Pagination
        public async Task<object> GetEmployeesPagedAsync(
            int pageNumber, int pageSize)
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
                    .Select(EmployeeMapper.ToResponseDto)
                    .ToList()
            };
        }
    }
}
