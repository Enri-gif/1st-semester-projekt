using api.DTOs;
using api.Models;
using Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[ApiController]
[Route ("api/[controller]")]
public class StudentsController : ControllerBase
{
    private readonly IStudentService studentService;

    public StudentsController (IStudentService studentService)
    {
        this.studentService = studentService;
    }

    [HttpPost]
    public async Task<ActionResult<Student>> CreateStudent ([FromBody] CreateStudentDTO dto)
    {
        var student = new Student
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName
        };

        var addSuccess = await studentService.AddStudent (student);

        if (!addSuccess)
        {
            Console.WriteLine ("Failed adding student");
            return BadRequest();
        }

        Console.WriteLine ($"Succesfully created student {student.FirstName}.");

        return CreatedAtAction (nameof (GetStudent), new { id = student.Id }, student);
    }

    [HttpGet ("{id}")]
    public async Task<ActionResult<Student>> GetStudent (int id)
    {
        var student = await studentService.GetStudent (id);

        if (student == null)
        {
            Console.WriteLine ($"Controller found no student for id {id}.");
            return NotFound ();
        }

        Console.WriteLine ($"Controller found a student for id {id}.");

        return student;
    }

    public async Task<ActionResult<Student>> UpdateStudent (Student student)
    {
        throw new NotImplementedException ("Not bad yet because TDD assignment.");
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteStudent (int id)
    {
        var deleteSuccess = await studentService.DeleteStudent (id);

        if (!deleteSuccess)
        {
            return BadRequest ();
        }

        return NoContent ();
    }
}
