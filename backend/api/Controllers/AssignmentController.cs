using Microsoft.AspNetCore.Mvc;
using api.Models;
using api.Services;
using api.DTOs;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssignmentController : ControllerBase{

        private readonly AssignmentService _assignmentservice;
        private readonly MongoImageService _mongoImageService;

        public AssignmentController(AssignmentService assignmentservice, MongoImageService mongoImageService){
            _mongoImageService = mongoImageService;
            _assignmentservice = assignmentservice;
        }

        [HttpGet]
        public async Task<ActionResult<List<Assignment>>> GetAssignments(){
            return Ok(await _assignmentservice.GetAll());
        }

        [HttpGet("with-images")]
        public async Task<ActionResult<List<AssignmentImagesDTO>>> GetAssignmentsWithImages(){
            var assignments = await _assignmentservice.GetAll();
            List<AssignmentImagesDTO> result = new List<AssignmentImagesDTO>();

            foreach (Assignment assignment in assignments){
                List<string> images = await _mongoImageService.GetImagesByAssignmentIdAsync(assignment.Id.ToString());

                result.Add(new AssignmentImagesDTO{
                    Assignment = assignment,
                    ImageUrls = images
                });
            }

            return Ok(result);
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<Assignment>> GetAssignmentById(Guid id){
            Assignment assignment = await _assignmentservice.GetById(id);
            if (assignment == null)
                return NotFound();

            return Ok(assignment);
        }

        [HttpGet("{id}/with-images")]
        public async Task<ActionResult<AssignmentImagesDTO>> GetAssignmentByIdWithImages(Guid id){
            Assignment assignment = await _assignmentservice.GetById(id);
            if (assignment == null)
                return NotFound();

            List<string> images = await _mongoImageService.GetImagesByAssignmentIdAsync(assignment.Id.ToString());

            var result = new AssignmentImagesDTO{
                Assignment = assignment,
                ImageUrls = images
            };

            return Ok(result);
        }

        [HttpGet("with-id")]
        public async Task<ActionResult<List<AssignmentWithIdDTO>>> GetAssignmentsWithId(){
            var assignments = await _assignmentservice.GetAll();
            return Ok(assignments.Select(a => new AssignmentWithIdDTO{
                Id = a.Id,
                Subject = a.Subject,
                Topic = a.Topic,
                Number = a.Number
            }).ToList());
        }


        [HttpPost]
        public async Task<ActionResult<Assignment>> CreateAssignment([FromForm] Assignment newAssignment, [FromForm] IFormFileCollection images){
            if (newAssignment == null)
                return BadRequest();

            Assignment createdAssignment = await _assignmentservice.Create(newAssignment);

            if (images != null && images.Count > 0){
                foreach (var file in images){

                    using var ms = new MemoryStream();
                    await file.CopyToAsync(ms);
                    var fileBytes = ms.ToArray();

                    await _mongoImageService.UploadImageAsync(
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
