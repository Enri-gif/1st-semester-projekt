using api.Models;

namespace api.Interfaces;

public interface IStudentService
{
    Task<Student?> GetStudent(Guid id);
    Task<bool> AddStudent(Student student);
    Task<bool> UpdateStudent(Student updateStudent);
    Task<bool> DeleteStudent(Guid id);
    Task<IEnumerable<Student>> GetByClass(string className);
}
