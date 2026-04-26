using api.Models;
using Microsoft.EntityFrameworkCore;
using api.Interfaces;

namespace api.Data;

public class AssignmentRepository : IAssignmentRepository
{
    private readonly ApplicationDbContext _db;

    public AssignmentRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<Assignment> AddAsync(Assignment assignment)
    {
        _db.Assignments.Add(assignment);
        await _db.SaveChangesAsync();
        return assignment;
    }

    public Task<Assignment?> GetByIdAsync(Guid id)
        => _db.Assignments.FindAsync(id).AsTask();

    public async Task<IEnumerable<Assignment>> GetAllAsync()
        => await _db.Assignments.AsNoTracking().ToListAsync();

    public async Task<bool> RemoveAsync(Assignment assignment)
    {
        _db.Assignments.Remove(assignment);
        await _db.SaveChangesAsync();
        return true;
    }

    // For query-building callers (filter + paging). Returns IQueryable so the
    // service layer can compose Where/OrderBy without exposing DbContext.
    public IQueryable<Assignment> Query() => _db.Assignments.AsNoTracking();

    public Task<int> SaveChangesAsync() => _db.SaveChangesAsync();
}
