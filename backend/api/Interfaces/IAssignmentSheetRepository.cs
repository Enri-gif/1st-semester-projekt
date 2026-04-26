using api.Models;

namespace api.Interfaces;

public interface IAssignmentSheetRepository
{
    Task<IEnumerable<AssignmentSheet>> GetAllAsync();
    Task<AssignmentSheet?> GetByIdAsync(Guid id);
    Task<AssignmentSheet> CreateAsync(AssignmentSheet sheet);
    Task<bool> UpdateAsync(AssignmentSheet sheet);
    Task<bool> DeleteAsync(Guid id);
    // Replaces the set of assignments attached to the sheet.
    // Assignments not in the new set have their AssignmentSheetId nulled; they are not deleted.
    Task SetAssignmentsAsync(Guid sheetId, IEnumerable<Guid> assignmentIds);
}
