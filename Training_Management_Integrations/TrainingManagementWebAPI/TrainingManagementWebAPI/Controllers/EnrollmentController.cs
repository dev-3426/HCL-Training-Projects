
using Microsoft.AspNetCore.Mvc;
using TrainingManagementWebAPI.Models;
using TrainingManagementWebAPI.Services;

namespace TrainingManagementWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EnrollmentController : ControllerBase
    {
        private readonly EnrollmentService service;

        public EnrollmentController(EnrollmentService service)
        {
            this.service = service;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(service.GetAllEnrollments());
        }

        [HttpPost]
        public IActionResult Enroll(Enrollment enrollment)
        {
            string result = service.Enroll(enrollment);

            if (!result.StartsWith("Enrollment successful."))
                return BadRequest(result);

            return Ok(result);
        }
    }
}
