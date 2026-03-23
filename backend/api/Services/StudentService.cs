using api.Data;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Services;

public interface IStudentService
{
    Task<Student?> GetStudent (int id);
    Task<bool> AddStudent (Student student);
    Task<Student> UpdateStudent (int id);
    Task<bool> DeleteStudent (int id);
}

public class StudentService : IStudentService
{
    private readonly ApplicationDbContext dbContext;

    public StudentService (ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<Student?> GetStudent (int id)
    {
        var student = await dbContext.Students.FirstOrDefaultAsync (s => s.Id == id);

        if (student == null)
        {
            // TODO Logging
        }

        return student;
    }

    public async Task<bool> AddStudent (Student student)
    {
        if (string.IsNullOrEmpty (student.FirstName))
        {
            // TODO Logging
            return false;
        }

        if (string.IsNullOrEmpty(student.LastName))
        {
            // TODO Logging
            return false;
        }

        dbContext.Students.Add (student);
        await dbContext.SaveChangesAsync ();

        return true;
    }

    public async Task<Student> UpdateStudent (int id)
    {
        throw new NotImplementedException ();
    }

    public async Task<bool> DeleteStudent (int id)
    {
        var student = await dbContext.Students.FirstOrDefaultAsync (s => s.Id == id);

        if (student == null)
        {
            // TODO Logging
            return false;
        }

        dbContext.Students.Remove (student);
        await dbContext.SaveChangesAsync ();

        // TODO Logging

        return true;
    }
}
