using Microsoft.AspNetCore.Mvc;
using api.Models;
using api.Services;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideoController : ControllerBase{

        private readonly MongoVideoService _mongoVideoService;

        public VideoController (MongoVideoService mongoVideoService){
            _mongoVideoService = mongoVideoService;
        }

        [HttpPost]
        [RequestSizeLimit(524288000)] // 500 MB
        public async Task<ActionResult> UploadVideo([FromForm] IFormFile file, [FromForm] List<string> assignmentIds){

            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            using MemoryStream memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);

            var fileBytes = memoryStream.ToArray();

            var videoId = await _mongoVideoService.UploadVideoAsync(fileBytes, file.FileName, assignmentIds ?? new List<string>());

            return Ok(new{
                id = videoId.ToString(),
                fileName = file.FileName,
                assignmentIds = assignmentIds ?? new List<string>()
            });
        }
    }
}

