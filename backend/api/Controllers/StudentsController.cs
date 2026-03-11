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
    public async Task<ActionResult<Student>> CreateStudent (CreateStudentDTO dto)
    {
        var student = new Student
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
        };

        dbContext.Students.Add (student);
        await dbContext.SaveChangesAsync ();

        return CreatedAtAction (nameof (GetStudent), new { id = student.Id }, student);
    }

    [HttpGet ("{id}")]
    public async Task<ActionResult<Student>> GetStudent (int id)
    {
        var student = await dbContext.Students.FindAsync (id);

        if (student == null)
            return NotFound ();

        return student;
    }
}
