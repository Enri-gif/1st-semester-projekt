using api.Models;

namespace api.Data;

public interface IAssignmentSheetRepository
{
    Task<IEnumerable<AssignmentSheet>> GetAllAsync();
    Task<AssignmentSheet?> GetByIdAsync(Guid id);
    Task<AssignmentSheet> CreateAsync(AssignmentSheet sheet);
    Task<bool> UpdateAsync(AssignmentSheet sheet);
    Task<bool> DeleteAsync(Guid id);
}
