using api.Models;

namespace api.Data;

public interface IAssignmentRepository
{
    Task<Assignment> AddAsync(Assignment assignment);
    Task<Assignment?> GetByIdAsync(Guid id);
    Task<IEnumerable<Assignment>> GetAllAsync();
    Task<bool> RemoveAsync(Assignment assignment);
    IQueryable<Assignment> Query();
    Task<int> SaveChangesAsync();
}
