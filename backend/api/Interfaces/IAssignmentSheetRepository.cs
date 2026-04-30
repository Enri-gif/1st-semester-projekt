using api.Data;
using api.Models;

namespace api.Data;

public interface IAssignmentSheetRepository : IRepository<AssignmentSheet>
{
    // Backwards-compatible aliases over the generic AddAsync/RemoveAsync.
    Task<AssignmentSheet> CreateAsync(AssignmentSheet sheet);
    Task<bool> UpdateAsync(AssignmentSheet sheet);
    Task<bool> DeleteAsync(Guid id);

    // Replaces the set of assignments attached to the sheet.
    // Assignments not in the new set have their AssignmentSheetId nulled; they are not deleted.
    Task SetAssignmentsAsync(Guid sheetId, IEnumerable<Guid> assignmentIds);
}
