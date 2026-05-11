namespace api.Data;

// Shared CRUD contract. Specific repositories extend this with entity-specific
// queries (e.g. AssignmentSheetRepository.SetAssignmentsAsync) instead of
// duplicating the basic Add/Get/Remove plumbing.
public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> AddAsync(T entity);
    Task<bool> RemoveAsync(T entity);
    IQueryable<T> Query();
    Task<int> SaveChangesAsync();
}
