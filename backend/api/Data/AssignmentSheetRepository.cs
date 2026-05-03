using api.Models;
using Microsoft.EntityFrameworkCore;
using api.Interfaces;

namespace api.Data;

public class AssignmentSheetRepository : Repository<AssignmentSheet>, IAssignmentSheetRepository
{
    public AssignmentSheetRepository(ApplicationDbContext db) : base(db)
    {
    }

    public override async Task<AssignmentSheet?> GetByIdAsync(Guid id)
    {
        return await _set
            .AsNoTracking()
            .Include(s => s.Assignments)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public Task<AssignmentSheet> CreateAsync(AssignmentSheet sheet) => AddAsync(sheet);

    public async Task<bool> UpdateAsync(AssignmentSheet sheet)
    {
        var existing = await _set.FindAsync(sheet.Id);
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
        existing.Topic = sheet.Topic;
        existing.Education = sheet.Education;
        existing.Tags = sheet.Tags ?? new List<string>();
        existing.Grade = sheet.Grade;
        existing.Feedback = sheet.Feedback;
        existing.CorrectionNotes = sheet.CorrectionNotes;

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var sheet = await _set.FindAsync(id);
        if (sheet == null)
        {
            return false;
        }
        return await RemoveAsync(sheet);
    }

    public async Task SetAssignmentsAsync(Guid sheetId, IEnumerable<Guid> assignmentIds)
    {
        var ids = assignmentIds?.ToHashSet() ?? new HashSet<Guid>();

        var currentlyAttached = await _db.Assignments
            .Where(a => a.AssignmentSheetId == sheetId)
            .ToListAsync();

        foreach (var a in currentlyAttached.Where(a => !ids.Contains(a.Id)))
        {
            a.AssignmentSheetId = null;
        }

        if (ids.Count > 0)
        {
            var toAttach = await _db.Assignments
                .Where(a => ids.Contains(a.Id))
                .ToListAsync();

            foreach (var a in toAttach)
            {
                a.AssignmentSheetId = sheetId;
            }
        }

        await _db.SaveChangesAsync();
    }
}
