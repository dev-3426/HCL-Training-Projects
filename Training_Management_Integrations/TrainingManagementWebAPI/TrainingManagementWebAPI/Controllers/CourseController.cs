
using Microsoft.AspNetCore.Mvc;
using TrainingManagementWebAPI.Services;

namespace TrainingManagementWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CourseController : ControllerBase
    {
        private readonly CourseService service;

        public CourseController(CourseService service)
        {
            this.service = service;
        }

        // GET: /api/course
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(service.GetAllCourses());
        }

        // GET: /api/course/CS101
        [HttpGet("{code}")]
        public IActionResult GetByCode(string code)
        {
            var course = service.SearchCourse(code);

            if (course == null)
            {
                return NotFound("Course not found");
            }

            return Ok(course);
        }

        // GET: /api/course/affordable
        [HttpGet("affordable")]
        public IActionResult GetAffordable()
        {
            return Ok(service.GetAffordableCourses());
        }

        // GET: /api/course/sort
        [HttpGet("sort")]
        public IActionResult SortByFee()
        {
            return Ok(service.SortCoursesByFee());
        }
    }
}
