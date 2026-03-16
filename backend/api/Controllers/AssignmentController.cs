using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using api.Models;
using api.Services;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssignmentController : ControllerBase{

        private readonly AssignmentService _service;

        public AssignmentController(AssignmentService service){
            _service = service;
        }

        // private static List<Assignment> assignments = new List<Assignment>{
        //     //test
        //     new Assignment{
        //         Answer = "testAnswer",
        //         Topic = "testTopic",
        //         Subject = "testSubject",
        //         Points = 1,
        //         Level = "A",
        //         Subtest = 1,
        //         Number = 1,
        //         Subquestion = "a"
        //     },
        //     new Assignment{
        //         Answer = "testAnswer2",
        //         Topic = "testTopic2",
        //         Subject = "testSubject2",
        //         Points = 2,
        //         Level = "B",
        //         Subtest = 2,
        //         Number = 2,
        //         Subquestion = "b"
        //     }
        // };

        [HttpGet]
        public async Task<ActionResult<List<Assignment>>> GetAssignments(){
            return Ok(await _service.GetAll());
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<Assignment>> GetAssignmentById(Guid id){
            Assignment assignment = await _service.GetById(id);
            if (assignment == null)
                return NotFound();

            return Ok(assignment);
            
        }

        [HttpPost]
        public async Task<ActionResult<Assignment>> CreateAssignment(Assignment newAssignment){
            if (newAssignment == null)
                return BadRequest();

            _service.Create(newAssignment);
            return CreatedAtAction(nameof(GetAssignmentById), new {id = newAssignment.Id}, newAssignment);
        }
    }
}
