using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Data;

public class StudentRepository : IStudentRepository
{
    private readonly ApplicationDbContext _db;

    public StudentRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public Task<Student?> GetByIdAsync(Guid id)
        => _db.Students.FirstOrDefaultAsync(s => s.Id == id);

    public async Task<IEnumerable<Student>> GetByClassAsync(string className)
    {
        if (string.IsNullOrWhiteSpace(className))
        {
            return Array.Empty<Student>();
        }
        return await _db.Students
            .AsNoTracking()
            .Where(s => s.StudentClass == className)
            .OrderBy(s => s.LastName)
            .ThenBy(s => s.FirstName)
            .ToListAsync();
    }

    public async Task AddAsync(Student student)
    {
        _db.Students.Add(student);
        await _db.SaveChangesAsync();
    }

    public async Task<bool> RemoveAsync(Student student)
    {
        _db.Students.Remove(student);
        await _db.SaveChangesAsync();
        return true;
    }

    public Task<int> SaveChangesAsync() => _db.SaveChangesAsync();
}
