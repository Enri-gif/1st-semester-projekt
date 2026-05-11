using Microsoft.AspNetCore.Mvc;
using api.Services;
using MongoDB.Bson;

namespace api.Controllers{
    [Route("api/[controller]")]
    [ApiController]
    public class VideoController : ControllerBase{
        private readonly MongoVideoService _mongoVideoService;

        public VideoController(MongoVideoService mongoVideoService){
            _mongoVideoService = mongoVideoService;
        }

        [HttpPost]
        [RequestSizeLimit(524288000)] // 500 MB
        public async Task<ActionResult> UploadVideo([FromForm] IFormFile file, [FromForm] List<string> assignmentIds){
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            using MemoryStream memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            byte[] fileBytes = memoryStream.ToArray();

            ObjectId videoId = await _mongoVideoService.UploadVideoAsync(
                fileBytes,
                file.FileName,
                assignmentIds ?? new List<string>()
            );

            return Ok(new{
                id = videoId.ToString(),
                fileName = file.FileName,
                assignmentIds = assignmentIds ?? new List<string>(),
                url = $"{Request.Scheme}://{Request.Host}/api/videos/{videoId}"
            });
        }

        [HttpGet("assignment/{id}")]
        public async Task<ActionResult<List<string>>> GetVideosByAssignmentId(string id){
            List<string> videoUrls = await _mongoVideoService.GetVideosByAssignmentIdAsync(id);
            return Ok(videoUrls);
        }

        [HttpGet]
        public async Task<ActionResult<List<string>>> GetAllVideos(){
            List<string> videoUrls = await _mongoVideoService.GetAllVideosAsync();
            return Ok(videoUrls);
        }

        [HttpGet("{id}/stream")]
        public async Task<IActionResult> StreamVideo(string id){
            if (!ObjectId.TryParse(id, out ObjectId objectId))
                return BadRequest("Invalid video ID.");

            byte[] videoBytes = await _mongoVideoService.DownloadVideoAsync(objectId);
            var fileInfo = await _mongoVideoService.GetFileInfoAsync(objectId);

            string contentType = "video/mp4";
            if (fileInfo != null && fileInfo.Metadata.Contains("contentType"))
                contentType = fileInfo.Metadata["contentType"].AsString;

            return File(videoBytes, contentType, fileInfo?.Filename ?? "video.mp4");
        }
    }
}
