using api.DTOs;
using api.Models;
using api.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared;
using api.Interfaces;

namespace api.Controllers;

[ApiController]
[Route ("api/[controller]")]
[Authorize(Roles = Roles.Teacher)]
public class StudentsController : ControllerBase
{
    private readonly IStudentService studentService;
    private readonly IValidator<CreateStudentDTO> validator;
    private readonly ILogger<StudentsController> logger;

    public StudentsController (IStudentService studentService, IValidator<CreateStudentDTO> validator, ILogger<StudentsController> logger)
    {
        this.studentService = studentService;
        this.validator = validator;
        this.logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<StudentResponseDTO>> CreateStudent ([FromBody] CreateStudentDTO dto)
    {
        var validation = await validator.ValidateAsync(dto);
        if (!validation.IsValid)
        {
            return ValidationProblem(new ValidationProblemDetails(
                validation.Errors.GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray())));
        }

        var student = new Student
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            CustomerNumber = dto.CustomerNumber,
            CustomerId = dto.CustomerId,
            Phone = dto.Phone,
            Address = dto.Address,
            City = dto.City,
            Note = dto.Note,
            StudentClass = dto.StudentClass,
            BirthDate = dto.BirthDate,
            Enrolled = dto.Enrolled,
            Inactive = dto.Inactive,
            Pause = dto.Pause
        };

        var addSuccess = await studentService.AddStudent (student);

        if (!addSuccess)
        {
            logger.LogWarning("Failed adding student {FirstName} {LastName}", student.FirstName, student.LastName);
            return BadRequest();
        }

        logger.LogInformation("Created student {StudentId} ({FirstName})", student.Id, student.FirstName);

        return CreatedAtAction (nameof (GetStudent), new { id = student.Id }, student.ToResponse());
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<StudentResponseDTO>>> GetStudents ([FromQuery(Name = "class")] string? className)
    {
        if (string.IsNullOrWhiteSpace(className))
        {
            return BadRequest(new { message = "Query parameter 'class' is required." });
        }

        var students = await studentService.GetByClass(className);
        return Ok(students.Select(s => s.ToResponse()));
    }

    [HttpGet ("{id}")]
    public async Task<ActionResult<StudentResponseDTO>> GetStudent (Guid id)
    {
        var student = await studentService.GetStudent (id);

        if (student == null)
        {
            logger.LogInformation("No student found for id {StudentId}", id);
            return NotFound ();
        }

        logger.LogDebug("Found student {StudentId}", id);

        return student.ToResponse();
    }

    [HttpPut()]
    public async Task<IActionResult> UpdateStudent ([FromBody] Student updatedStudent)
    {
        var currentStudent = await studentService.GetStudent (updatedStudent.Id);

        if (currentStudent == null)
        {
            return BadRequest ("No student found.");
        }

        await studentService.UpdateStudent (updatedStudent);

        return NoContent ();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteStudent (Guid id)
    {
        var deleteSuccess = await studentService.DeleteStudent (id);

        if (!deleteSuccess)
        {
            return NotFound ();
        }

        return NoContent ();
    }
}
