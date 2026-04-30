using api.Models;

namespace api.Data;

public interface IStudentRepository : IRepository<Student>
{
    Task<IEnumerable<Student>> GetByClassAsync(string className);
}
