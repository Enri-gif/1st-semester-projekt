using api.DTOs;
using api.Models;
using Shared.Contracts;

namespace api.Interfaces;

public interface IAssignmentService
{
    Task<Assignment> Create(Assignment assignment);
    Task<Assignment?> GetById(Guid id);
    Task<IEnumerable<Assignment>> GetAll();
    Task<bool> DeleteAssignment(Guid id);
    Task<PagedResult<Assignment>> GetFiltered(
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
        string? sortDir);
}
