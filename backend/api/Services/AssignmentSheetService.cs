using api.Data;
using api.DTOs;
using api.Models;

namespace api.Services;

public interface IAssignmentSheetService
{
    Task<IEnumerable<AssignmentSheet>> GetAllAssignmentSheets();
    Task<AssignmentSheet?> GetAssignmentSheet(Guid id);
    Task<AssignmentSheet> CreateAssignmentSheet(AssignmentSheet sheet);
    Task<bool> UpdateAssignmentSheet(AssignmentSheet sheet);
    Task<bool> DeleteAssignmentSheet(Guid id);
    Task<AssignmentSheetPointsDto?> GetPointsBreakdown(Guid sheetId);
}

public class AssignmentSheetService : IAssignmentSheetService
{
    private readonly IAssignmentSheetRepository _repo;

    public AssignmentSheetService(IAssignmentSheetRepository repo)
    {
        _repo = repo;
    }

    public async Task<IEnumerable<AssignmentSheet>> GetAllAssignmentSheets()
    {
        return await _repo.GetAllAsync();
    }

    public async Task<AssignmentSheet?> GetAssignmentSheet(Guid id)
    {
        return await _repo.GetByIdAsync(id);
    }

    public async Task<AssignmentSheet> CreateAssignmentSheet(AssignmentSheet sheet)
    {
        return await _repo.CreateAsync(sheet);
    }

    public async Task<bool> UpdateAssignmentSheet(AssignmentSheet sheet)
    {
        return await _repo.UpdateAsync(sheet);
    }

    public async Task<bool> DeleteAssignmentSheet(Guid id)
    {
        return await _repo.DeleteAsync(id);
    }

    public async Task<AssignmentSheetPointsDto?> GetPointsBreakdown(Guid sheetId)
    {
        var sheet = await _repo.GetByIdAsync(sheetId);
        if (sheet == null)
        {
            return null;
        }

        var perAssignment = sheet.Assignments
            .Select(a => new AssignmentPointsDto
            {
                AssignmentId = a.Id,
                Number = a.Number,
                Points = a.Points
            })
            .ToList();

        return new AssignmentSheetPointsDto
        {
            AssignmentSheetId = sheet.Id,
            TotalPoints = perAssignment.Sum(p => p.Points),
            PerAssignment = perAssignment
        };
    }
}
