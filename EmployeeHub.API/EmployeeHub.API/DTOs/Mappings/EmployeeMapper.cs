
using EmployeeHub.API.Models;

namespace EmployeeHub.API.DTOs.Mappings
{
    public static class EmployeeMapper
    {
        // Employee to Response DTO
        public static EmployeeResponseDto ToResponseDto(Employee employee)
        {
            return new EmployeeResponseDto
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Email = employee.Email,
                IsActive = employee.IsActive
            };
        }

        // Create DTO to Employee
        public static Employee ToEmployee(CreateEmployeeDto dto)
        {
            return new Employee
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Salary = dto.Salary,
                DateOfJoining = dto.DateOfJoining,
                IsActive = dto.IsActive
            };
        }

        // Update existing Employee from DTO
        public static void UpdateEmployeeFromDto(
            Employee employee,
            UpdateEmployeeDto dto)
        {
            employee.FirstName = dto.FirstName;
            employee.LastName = dto.LastName;
            employee.Email = dto.Email;
            employee.PhoneNumber = dto.PhoneNumber;
            employee.Salary = dto.Salary;
            employee.DateOfJoining = dto.DateOfJoining;
            employee.IsActive = dto.IsActive;
        }
    }
}
