using api.Data;
using api.Models;
using Microsoft.Extensions.Logging;
using api.Interfaces;

namespace api.Services;

public class StudentService : IStudentService
{
    private readonly IStudentRepository repo;
    private readonly ILogger<StudentService> logger;

    public StudentService(IStudentRepository repo, ILogger<StudentService> logger)
    {
        this.repo = repo;
        this.logger = logger;
    }

    public async Task<Student?> GetStudent(Guid id)
    {
        var student = await repo.GetByIdAsync(id);
        if (student == null)
        {
            logger.LogInformation("GetStudent: no student found for id {StudentId}", id);
        }
        return student;
    }

    public async Task<bool> AddStudent(Student student)
    {
        if (string.IsNullOrEmpty(student.FirstName))
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

        await repo.AddAsync(student);
        logger.LogInformation("AddStudent: saved student {StudentId}", student.Id);
        return true;
    }

    public async Task<bool> UpdateStudent(Student updateStudent)
    {
        var student = await repo.GetByIdAsync(updateStudent.Id);
        if (student == null)
        {
            logger.LogInformation("UpdateStudent: no student found for id {StudentId}", updateStudent.Id);
            return false;
        }

        student.FirstName = updateStudent.FirstName;
        student.LastName = updateStudent.LastName;
        student.Email = updateStudent.Email;

        await repo.SaveChangesAsync();
        logger.LogInformation("UpdateStudent: updated student {StudentId}", student.Id);
        return true;
    }

    public Task<IEnumerable<Student>> GetByClass(string className) => repo.GetByClassAsync(className);

    public async Task<bool> DeleteStudent(Guid id)
    {
        var student = await repo.GetByIdAsync(id);
        if (student == null)
        {
            logger.LogInformation("DeleteStudent: no student found for id {StudentId}", id);
            return false;
        }

        await repo.RemoveAsync(student);
        logger.LogInformation("DeleteStudent: deleted student {StudentId}", id);
        return true;
    }
}
