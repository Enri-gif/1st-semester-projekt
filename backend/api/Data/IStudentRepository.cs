using api.Models;

namespace api.Data;

public interface IStudentRepository
{
    Task<Student?> GetByIdAsync(Guid id);
    Task<IEnumerable<Student>> GetByClassAsync(string className);
    Task AddAsync(Student student);
    Task<bool> RemoveAsync(Student student);
    Task<int> SaveChangesAsync();
}
