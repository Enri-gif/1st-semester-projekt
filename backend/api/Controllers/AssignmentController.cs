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
    private readonly AssignmentService _service;
    private readonly MongoAttachmentService _mongoAttachmentService;
    private readonly IValidator<CreateAssignmentDto> _validator;

    public AssignmentController(AssignmentService service, MongoAttachmentService mongoAttachmentService, IValidator<CreateAssignmentDto> validator)
    {
        _mongoAttachmentService = mongoAttachmentService;
        _service = service;
        _validator = validator;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<Assignment>>> GetAssignments(
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
        var result = await _service.GetFiltered(
            subject, level, year, topic, owner, tag, assignmentSheetId,
            page, pageSize, sortBy, sortDir);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Assignment>> GetAssignmentById(Guid id)
    {
        Assignment? assignment = await _service.GetById(id);
        if (assignment == null)
            return NotFound();

        return Ok(assignment);
    }

    [HttpPost]
    public async Task<ActionResult<Assignment>> CreateAssignment(
        [FromForm] CreateAssignmentDto dto,
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

        var created = await _service.Create(newAssignment);

        if (images != null && images.Count > 0)
        {
            foreach (var file in images)
            {
                using var ms = new MemoryStream();
                await file.CopyToAsync(ms);
                var fileBytes = ms.ToArray();

                await _mongoAttachmentService.UploadImageAsync(
                    fileBytes,
                    file.FileName,
                    created.Id.ToString()
                );
            }
        }

        return CreatedAtAction(
            nameof(GetAssignmentById),
            new { id = created.Id },
            created
        );
    }
}
