
namespace EmployeeHub.API.Models
{
    public class Employee
    {
        public int Id { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public decimal Salary { get; set; }

        public DateTime DateOfJoining { get; set; }

        public bool IsActive { get; set; }
    }
}
