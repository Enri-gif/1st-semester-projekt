using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Data;

public class AssignmentSheetRepository : IAssignmentSheetRepository
{
    private readonly ApplicationDbContext _dbContext;

    public AssignmentSheetRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<AssignmentSheet>> GetAllAsync()
    {
        return await _dbContext.AssignmentSheets
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<AssignmentSheet?> GetByIdAsync(Guid id)
    {
        return await _dbContext.AssignmentSheets
            .AsNoTracking()
            .Include(s => s.Assignments)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<AssignmentSheet> CreateAsync(AssignmentSheet sheet)
    {
        _dbContext.AssignmentSheets.Add(sheet);
        await _dbContext.SaveChangesAsync();
        return sheet;
    }

    public async Task<bool> UpdateAsync(AssignmentSheet sheet)
    {
        var existing = await _dbContext.AssignmentSheets.FindAsync(sheet.Id);
        if (existing == null)
        {
            return false;
        }

        existing.Title = sheet.Title;
        existing.Subject = sheet.Subject;
        existing.Level = sheet.Level;
        existing.Year = sheet.Year;
        existing.Owner = sheet.Owner;
        existing.Type = sheet.Type;

        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var sheet = await _dbContext.AssignmentSheets.FindAsync(id);
        if (sheet == null)
        {
            return false;
        }

        _dbContext.AssignmentSheets.Remove(sheet);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task SetAssignmentsAsync(Guid sheetId, IEnumerable<Guid> assignmentIds)
    {
        var ids = assignmentIds?.ToHashSet() ?? new HashSet<Guid>();

        // Null the FK on assignments that were attached to this sheet but are no longer in the set
        var currentlyAttached = await _dbContext.Assignments
            .Where(a => a.AssignmentSheetId == sheetId)
            .ToListAsync();

        foreach (var a in currentlyAttached.Where(a => !ids.Contains(a.Id)))
        {
            a.AssignmentSheetId = null;
        }

        // Attach newly-selected assignments that weren't already attached elsewhere to this sheet
        if (ids.Count > 0)
        {
            var toAttach = await _dbContext.Assignments
                .Where(a => ids.Contains(a.Id))
                .ToListAsync();

            foreach (var a in toAttach)
            {
                a.AssignmentSheetId = sheetId;
            }
        }

        await _dbContext.SaveChangesAsync();
    }
}
