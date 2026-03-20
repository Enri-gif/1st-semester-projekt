using Microsoft.AspNetCore.Mvc;
using api.Models;
using api.Services;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssignmentController : ControllerBase{

        private readonly AssignmentService _service;
        private readonly MongoAttachmentService _mongoAttachmentService;

        public AssignmentController(AssignmentService service, MongoAttachmentService mongoAttachmentService){
            _mongoAttachmentService = mongoAttachmentService;
            _service = service;
        }

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
        public async Task<ActionResult<Assignment>> CreateAssignment([FromForm] Assignment newAssignment, [FromForm] IFormFileCollection images){
            if (newAssignment == null)
                return BadRequest();

            Assignment createdAssignment = await _service.Create(newAssignment);

            if (images != null && images.Count > 0){
                foreach (var file in images){

                    using var ms = new MemoryStream();
                    await file.CopyToAsync(ms);
                    var fileBytes = ms.ToArray();

                    await _mongoAttachmentService.UploadImageAsync(
                        fileBytes,
                        file.FileName,
                        createdAssignment.Id.ToString()
                    );
                }
            }

            return CreatedAtAction(
                nameof(GetAssignmentById),
                new { id = newAssignment.Id },
                newAssignment
            );
        }
    }
}
