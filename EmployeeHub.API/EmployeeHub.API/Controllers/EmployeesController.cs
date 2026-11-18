
using EmployeeHub.API.DTOs;
using EmployeeHub.API.Models;
using EmployeeHub.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeHub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeesController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        // GET: api/employees
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeResponseDto>>> GetEmployees()
        {
            var employees = await _employeeService.GetAllEmployeesAsync();

            return Ok(employees);
        }

        // GET: api/employees/1
        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeResponseDto>> GetEmployeeById(int id)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(id);

            if (employee == null)
            {
                return NotFound();
            }

            return Ok(employee);
        }

        // POST: api/employees
        [HttpPost]
        public async Task<ActionResult<EmployeeResponseDto>> CreateEmployee(
            CreateEmployeeDto dto)
        {
            var createdEmployee = await _employeeService.CreateEmployeeAsync(dto);

            return CreatedAtAction(
                nameof(GetEmployeeById),
                new { id = createdEmployee.Id },
                createdEmployee);
        }

        // PUT: api/employees/1
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(
            int id, UpdateEmployeeDto dto)
        {
            var updated = await _employeeService.UpdateEmployeeAsync(id, dto);

            if (!updated)
            {
                return NotFound();
            }

            return NoContent();
        }

        // DELETE: api/employees/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var deleted = await _employeeService.DeleteEmployeeAsync(id);

            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }

        // GET: api/employees/search
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<EmployeeResponseDto>>> SearchEmployees(
            string? name, bool? isActive)
        {
            var employees = await _employeeService.SearchEmployeesAsync(
                name, isActive);

            return Ok(employees);
        }

        // GET: api/employees/paged
        [HttpGet("paged")]
        public async Task<IActionResult> GetEmployeesPaged(
            int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber < 1 || pageSize < 1 || pageSize > 100)
            {
                return BadRequest("Invalid page number or page size.");
            }

            var result = await _employeeService.GetEmployeesPagedAsync(
                pageNumber, pageSize);

            return Ok(result);
        }
    }
}
