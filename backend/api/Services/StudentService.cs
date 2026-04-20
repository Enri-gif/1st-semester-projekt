using api.Data;
using api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace api.Services;

public interface IStudentService
{
    Task<Student?> GetStudent (Guid id);
    Task<bool> AddStudent (Student student);
    Task<bool> DeleteStudent (Guid id);
}

public class StudentService : IStudentService
{
    private readonly ApplicationDbContext dbContext;
    private readonly ILogger<StudentService> logger;

    public StudentService (ApplicationDbContext dbContext, ILogger<StudentService> logger)
    {
        this.dbContext = dbContext;
        this.logger = logger;
    }

    public async Task<Student?> GetStudent (Guid id)
    {
        var student = await dbContext.Students.FirstOrDefaultAsync (s => s.Id == id);

        if (student == null)
        {
            logger.LogInformation("GetStudent: no student found for id {StudentId}", id);
        }

        return student;
    }

    public async Task<bool> AddStudent (Student student)
    {
        if (string.IsNullOrEmpty (student.FirstName))
        {
            logger.LogWarning("AddStudent rejected: FirstName is empty");
            return false;
        }

        if (string.IsNullOrEmpty(student.LastName))
        {
            logger.LogWarning("AddStudent rejected: LastName is empty");
            return false;
        }

        if (student.Id == Guid.Empty)
        {
            student.Id = Guid.NewGuid();
        }

        dbContext.Students.Add (student);
        await dbContext.SaveChangesAsync ();

        logger.LogInformation("AddStudent: saved student {StudentId}", student.Id);
        return true;
    }

    public async Task<bool> DeleteStudent (Guid id)
    {
        var student = await dbContext.Students.FirstOrDefaultAsync (s => s.Id == id);

        if (student == null)
        {
            logger.LogInformation("DeleteStudent: no student found for id {StudentId}", id);
            return false;
        }

        dbContext.Students.Remove (student);
        await dbContext.SaveChangesAsync ();

        logger.LogInformation("DeleteStudent: deleted student {StudentId}", id);
        return true;
    }
}
