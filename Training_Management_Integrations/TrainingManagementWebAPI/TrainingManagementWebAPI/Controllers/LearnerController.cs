
using Microsoft.AspNetCore.Mvc;
using TrainingManagementWebAPI.Models;
using TrainingManagementWebAPI.Services;

namespace TrainingManagementWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LearnerController : ControllerBase
    {
        private readonly LearnerService service;

        public LearnerController(LearnerService service)
        {
            this.service = service;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(service.GetAllLearners());
        }

        [HttpGet("{id}")]
        public IActionResult GetById(string id)
        {
            var learner = service.SearchLearner(id);

            if (learner == null)
                return NotFound("Learner not found.");

            return Ok(learner);
        }

        [HttpPost]
        public IActionResult Add(Learner learner)
        {
            string result = service.AddLearner(learner);

            if (result != "Learner registered successfully.")
                return BadRequest(result);

            return Ok(result);
        }
    }
}
