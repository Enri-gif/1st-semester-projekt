using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using api.DTOs;
using api.Models;
using api.Services;
using Shared;

namespace api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = Roles.Teacher)]
public class AssignmentController : ControllerBase
{
    private readonly IAssignmentService _assignmentService;
    private readonly MongoImageService _mongoImageService;
    private readonly IValidator<CreateAssignmentDTO> _validator;

    public AssignmentController(IAssignmentService assignmentService, MongoImageService mongoImageService, IValidator<CreateAssignmentDTO> validator)
    {
        _mongoImageService = mongoImageService;
        _assignmentService = assignmentService;
        _validator = validator;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<AssignmentResponseDTO>>> GetAssignments(
        [FromQuery] string? subject,
        [FromQuery] string? level,
        [FromQuery] int? year,
        [FromQuery] string? topic,
        [FromQuery] string? owner,
        [FromQuery] string? tag,
        [FromQuery] Guid? assignmentSheetId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? sortBy = null,
        [FromQuery] string? sortDir = null)
    {
        var result = await _assignmentService.GetFiltered(
            subject, level, year, topic, owner, tag, assignmentSheetId,
            page, pageSize, sortBy, sortDir);

        return Ok(new PagedResult<AssignmentResponseDTO>
        {
            Items = result.Items.Select(a => a.ToResponse()).ToList(),
            Page = result.Page,
            PageSize = result.PageSize,
            Total = result.Total
        });
    }

    [HttpGet("with-images")]
    public async Task<ActionResult<List<AssignmentImagesDTO>>> GetAssignmentsWithImages()
    {
        var assignments = await _assignmentService.GetAll();
        var result = new List<AssignmentImagesDTO>();

        foreach (var assignment in assignments)
        {
            var images = await _mongoImageService.GetImagesByAssignmentIdAsync(assignment.Id.ToString());
            result.Add(new AssignmentImagesDTO
            {
                Assignment = assignment.ToResponse(),
                ImageUrls = images
            });
        }

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AssignmentResponseDTO>> GetAssignmentById(Guid id)
    {
        var assignment = await _assignmentService.GetById(id);
        if (assignment == null)
            return NotFound();

        return Ok(assignment.ToResponse());
    }

    [HttpGet("{id}/with-images")]
    public async Task<ActionResult<AssignmentImagesDTO>> GetAssignmentByIdWithImages(Guid id)
    {
        var assignment = await _assignmentService.GetById(id);
        if (assignment == null)
            return NotFound();

        var images = await _mongoImageService.GetImagesByAssignmentIdAsync(assignment.Id.ToString());
        return Ok(new AssignmentImagesDTO
        {
            Assignment = assignment.ToResponse(),
            ImageUrls = images
        });
    }

    [HttpGet("with-id")]
    public async Task<ActionResult<List<AssignmentWithIdDTO>>> GetAssignmentsWithId()
    {
        var assignments = await _assignmentService.GetAll();
        return Ok(assignments.Select(a => new AssignmentWithIdDTO
        {
            Id = a.Id,
            Subject = a.Subject,
            Topic = a.Topic,
            Number = a.Number
        }).ToList());
    }

    [HttpPost]
    public async Task<ActionResult<AssignmentResponseDTO>> CreateAssignment(
        [FromForm] CreateAssignmentDTO dto,
        [FromForm] IFormFileCollection? images)
    {
        var validation = await _validator.ValidateAsync(dto);
        if (!validation.IsValid)
        {
            return ValidationProblem(new ValidationProblemDetails(
                validation.Errors.GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray())));
        }

        var newAssignment = new Assignment
        {
            Id = Guid.NewGuid(),
            Education = dto.Education,
            Subject = dto.Subject,
            Level = dto.Level,
            Year = dto.Year,
            Date = dto.Date,
            Subquestion = dto.Subquestion,
            Subtest = dto.Subtest,
            Topic = dto.Topic,
            Answer = dto.Answer,
            Owner = dto.Owner,
            Number = dto.Number,
            Points = dto.Points,
            Tags = string.IsNullOrWhiteSpace(dto.Tags)
                ? new List<string>()
                : dto.Tags.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList()
        };

        var created = await _assignmentService.Create(newAssignment);

        if (images != null && images.Count > 0)
        {
            foreach (var file in images)
            {
                using var ms = new MemoryStream();
                await file.CopyToAsync(ms);
                var fileBytes = ms.ToArray();

                await _mongoImageService.UploadImageAsync(
                    fileBytes,
                    file.FileName,
                    created.Id.ToString()
                );
            }
        }

        return CreatedAtAction(
            nameof(GetAssignmentById),
            new { id = created.Id },
            created.ToResponse()
        );
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAssignment(Guid id)
    {
        bool deleted = await _assignmentService.DeleteAssignment(id);
        if (!deleted)
            return NotFound();

        return NoContent();
    }
}
