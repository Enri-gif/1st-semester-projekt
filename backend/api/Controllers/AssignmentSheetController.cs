using api.DTOs;
using api.Models;
using api.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared;

namespace api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = Roles.Teacher)]
public class AssignmentSheetController : ControllerBase
{
    private readonly IAssignmentSheetService _service;
    private readonly SpreadsheetService _spreadsheetService;
    private readonly IStudentService _studentService;
    private readonly IValidator<CreateAssignmentSheetDto> _createValidator;
    private readonly IValidator<UpdateAssignmentSheetDto> _updateValidator;

    public AssignmentSheetController(
        IAssignmentSheetService service,
        SpreadsheetService spreadsheetService,
        IStudentService studentService,
        IValidator<CreateAssignmentSheetDto> createValidator,
        IValidator<UpdateAssignmentSheetDto> updateValidator)
    {
        _service = service;
        _spreadsheetService = spreadsheetService;
        _studentService = studentService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AssignmentSheetResponseDto>>> GetAssignmentSheets()
    {
        var sheets = await _service.GetAllAssignmentSheets();
        return Ok(sheets.Select(s => s.ToResponse()));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AssignmentSheetResponseDto>> GetAssignmentSheet(Guid id)
    {
        var sheet = await _service.GetAssignmentSheet(id);
        if (sheet == null)
        {
            return NotFound();
        }

        return Ok(sheet.ToResponse());
    }

    [HttpPost]
    public async Task<ActionResult<AssignmentSheetResponseDto>> CreateAssignmentSheet([FromBody] CreateAssignmentSheetDto dto)
    {
        var validation = await _createValidator.ValidateAsync(dto);
        if (!validation.IsValid)
        {
            return ValidationProblem(new ValidationProblemDetails(
                validation.Errors.GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray())));
        }

        var sheet = new AssignmentSheet
        {
            Id = Guid.NewGuid(),
            Title = dto.Title,
            Subject = dto.Subject,
            Level = dto.Level,
            Year = dto.Year,
            Owner = dto.Owner
        };

        var created = await _service.CreateAssignmentSheet(sheet);
        return CreatedAtAction(nameof(GetAssignmentSheet), new { id = created.Id }, created.ToResponse());
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateAssignmentSheet(Guid id, [FromBody] UpdateAssignmentSheetDto dto)
    {
        var validation = await _updateValidator.ValidateAsync(dto);
        if (!validation.IsValid)
        {
            return ValidationProblem(new ValidationProblemDetails(
                validation.Errors.GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray())));
        }

        var sheet = new AssignmentSheet
        {
            Id = id,
            Title = dto.Title,
            Subject = dto.Subject,
            Level = dto.Level,
            Year = dto.Year,
            Owner = dto.Owner
        };

        var updated = await _service.UpdateAssignmentSheet(sheet);
        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAssignmentSheet(Guid id)
    {
        var deleted = await _service.DeleteAssignmentSheet(id);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpGet("{id}/points")]
    public async Task<ActionResult<AssignmentSheetPointsDto>> GetPointsBreakdown(Guid id)
    {
        var breakdown = await _service.GetPointsBreakdown(id);
        if (breakdown == null)
        {
            return NotFound();
        }

        return Ok(breakdown);
    }

    [HttpGet("{id}/spreadsheet")]
    public async Task<IActionResult> GetSpreadsheet(Guid id, [FromQuery] bool marking = false)
    {
        var sheet = await _service.GetAssignmentSheet(id);
        if (sheet == null)
        {
            return NotFound();
        }

        var bytes = _spreadsheetService.GenerateAssignmentSheetSpreadsheet(sheet, marking);
        var suffix = marking ? "retteark" : "regneark";
        var safeTitle = string.IsNullOrWhiteSpace(sheet.Title) ? "opgavesaet" : sheet.Title;
        var filename = $"{safeTitle}-{suffix}.xlsx";

        return File(bytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            filename);
    }

    [HttpGet("{id}/spreadsheet/class/{className}")]
    public async Task<IActionResult> GetClassMarkingSheets(Guid id, string className)
    {
        var sheet = await _service.GetAssignmentSheet(id);
        if (sheet == null)
        {
            return NotFound(new { message = "Assignment sheet not found." });
        }

        var students = (await _studentService.GetByClass(className)).ToList();
        if (students.Count == 0)
        {
            return NotFound(new { message = $"No students found for class '{className}'." });
        }

        var bytes = _spreadsheetService.GenerateClassMarkingSheetsZip(sheet, students);
        var safeTitle = string.IsNullOrWhiteSpace(sheet.Title) ? "opgavesaet" : sheet.Title;
        var safeClass = string.IsNullOrWhiteSpace(className) ? "klasse" : className;
        var filename = $"{safeTitle}-{safeClass}-rettearker.zip";

        return File(bytes, "application/zip", filename);
    }
}
