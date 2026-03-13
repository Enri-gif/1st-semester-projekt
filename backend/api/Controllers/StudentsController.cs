using api.Data;
using api.DTOs;
using api.Models;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[ApiController]
[Route ("api/[controller]")]
public class StudentsController : ControllerBase
{
    private readonly ApplicationDbContext dbContext;

    public StudentsController (ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    [HttpPost]
    public async Task<ActionResult<Student>> CreateStudent ([FromBody] CreateStudentDTO dto)
    {
        var student = new Student
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName
        };

        dbContext.Students.Add (student);
        await dbContext.SaveChangesAsync ();

        Console.WriteLine ($"Succesfully created student {student.FirstName}.");

        return CreatedAtAction (nameof (GetStudent), new { id = student.Id }, student);
    }

    [HttpGet ("{id}")]
    public async Task<ActionResult<Student>> GetStudent (int id)
    {
        var student = await dbContext.Students.FindAsync (id);

        if (student == null)
        {
            Console.WriteLine ($"Controller found no student for id {id}.");
            return NotFound ();
        }

        Console.WriteLine ($"Controller found a student for id {id}.");

        return student;
    }
}
