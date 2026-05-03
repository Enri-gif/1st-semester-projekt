using api.Models;
using Microsoft.EntityFrameworkCore;
using api.Interfaces;

namespace api.Data;

public class StudentRepository : Repository<Student>, IStudentRepository
{
    public StudentRepository(ApplicationDbContext db) : base(db)
    {
    }

    public override Task<Student?> GetByIdAsync(Guid id)
        => _set.FirstOrDefaultAsync(s => s.Id == id);

    public async Task<IEnumerable<Student>> GetByClassAsync(string className)
    {
        if (string.IsNullOrWhiteSpace(className))
        {
            return Array.Empty<Student>();
        }
        return await _set
            .AsNoTracking()
            .Where(s => s.StudentClass == className)
            .OrderBy(s => s.LastName)
            .ThenBy(s => s.FirstName)
            .ToListAsync();
    }
}
