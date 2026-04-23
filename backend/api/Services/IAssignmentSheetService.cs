using api.DTOs;
using api.Models;

namespace api.Services;

public interface IAssignmentSheetService
{
    Task<IEnumerable<AssignmentSheet>> GetAllAssignmentSheets();
    Task<AssignmentSheet?> GetAssignmentSheet(Guid id);
    Task<AssignmentSheet> CreateAssignmentSheet(AssignmentSheet sheet, IEnumerable<Guid>? assignmentIds = null);
    Task<bool> UpdateAssignmentSheet(AssignmentSheet sheet, IEnumerable<Guid>? assignmentIds = null);
    Task<bool> DeleteAssignmentSheet(Guid id);
    Task<AssignmentSheetPointsDTO?> GetPointsBreakdown(Guid sheetId);
}
