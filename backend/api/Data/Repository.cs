using Microsoft.EntityFrameworkCore;

namespace api.Data;

// Generic CRUD base. Holds the DbContext + DbSet<T> and implements the
// shared IRepository<T> surface. Entity-specific repositories derive from
// this and add the bespoke queries they need.
public class Repository<T> : IRepository<T> where T : class
{
    protected readonly ApplicationDbContext _db;
    protected readonly DbSet<T> _set;

    public Repository(ApplicationDbContext db)
    {
        _db = db;
        _set = db.Set<T>();
    }

    public virtual Task<T?> GetByIdAsync(Guid id)
        => _set.FindAsync(id).AsTask();

    public virtual async Task<IEnumerable<T>> GetAllAsync()
        => await _set.AsNoTracking().ToListAsync();

    public virtual async Task<T> AddAsync(T entity)
    {
        _set.Add(entity);
        await _db.SaveChangesAsync();
        return entity;
    }

    public virtual async Task<bool> RemoveAsync(T entity)
    {
        _set.Remove(entity);
        await _db.SaveChangesAsync();
        return true;
    }

    public virtual IQueryable<T> Query() => _set.AsNoTracking();

    public Task<int> SaveChangesAsync() => _db.SaveChangesAsync();
}
