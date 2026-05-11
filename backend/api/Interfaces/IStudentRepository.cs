using api.Data;
using api.Models;

namespace api.Interfaces;

public interface IStudentRepository : IRepository<Student>
{
    Task<IEnumerable<Student>> GetByClassAsync(string className);
}
