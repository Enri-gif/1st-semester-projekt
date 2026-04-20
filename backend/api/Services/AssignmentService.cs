using api.Data;
using api.DTOs;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Services;

public class AssignmentService
{
    private readonly ApplicationDbContext _dbcontext;

    public AssignmentService(ApplicationDbContext dbContext){
        _dbcontext = dbContext;
    }

    public async Task<Assignment> Create(Assignment assignment){

        _dbcontext.Assignments.Add(assignment);
        await _dbcontext.SaveChangesAsync(new CancellationToken());

        return assignment;
    }

    public async Task<Assignment?> GetById(Guid id){
        return await _dbcontext.Assignments.FindAsync(id);
    }

    public async Task<PagedResult<Assignment>> GetFiltered(
        string? subject,
        string? level,
        int? year,
        string? topic,
        string? owner,
        string? tag,
        Guid? assignmentSheetId,
        int page,
        int pageSize,
        string? sortBy,
        string? sortDir)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 20;
        if (pageSize > 100) pageSize = 100;

        IQueryable<Assignment> query = _dbcontext.Assignments.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(subject))
            query = query.Where(a => a.Subject == subject);
        if (!string.IsNullOrWhiteSpace(level))
            query = query.Where(a => a.Level == level);
        if (year.HasValue)
            query = query.Where(a => a.Year == year.Value);
        if (!string.IsNullOrWhiteSpace(topic))
            query = query.Where(a => a.Topic == topic);
        if (!string.IsNullOrWhiteSpace(owner))
            query = query.Where(a => a.Owner == owner);
        if (!string.IsNullOrWhiteSpace(tag))
            query = query.Where(a => a.Tags.Contains(tag));
        if (assignmentSheetId.HasValue)
            query = query.Where(a => a.AssignmentSheetId == assignmentSheetId.Value);

        bool descending = string.Equals(sortDir, "desc", StringComparison.OrdinalIgnoreCase);
        query = (sortBy?.ToLowerInvariant()) switch
        {
            "year"    => descending ? query.OrderByDescending(a => a.Year)    : query.OrderBy(a => a.Year),
            "points"  => descending ? query.OrderByDescending(a => a.Points)  : query.OrderBy(a => a.Points),
            "number"  => descending ? query.OrderByDescending(a => a.Number)  : query.OrderBy(a => a.Number),
            "subject" => descending ? query.OrderByDescending(a => a.Subject) : query.OrderBy(a => a.Subject),
            "level"   => descending ? query.OrderByDescending(a => a.Level)   : query.OrderBy(a => a.Level),
            "topic"   => descending ? query.OrderByDescending(a => a.Topic)   : query.OrderBy(a => a.Topic),
            "owner"   => descending ? query.OrderByDescending(a => a.Owner)   : query.OrderBy(a => a.Owner),
            _         => descending ? query.OrderByDescending(a => a.Date)    : query.OrderBy(a => a.Date),
        };

        int total = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Assignment>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            Total = total
        };
    }
}
