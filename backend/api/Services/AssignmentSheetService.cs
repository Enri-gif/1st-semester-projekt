using api.Data;
using api.DTOs;
using api.Models;
using api.Interfaces;

namespace api.Services;

public class AssignmentSheetService : IAssignmentSheetService
{
    private readonly IAssignmentSheetRepository _repo;
    private readonly IAssignmentRepository _assignmentRepo;

    public AssignmentSheetService(IAssignmentSheetRepository repo, IAssignmentRepository assignmentRepo)
    {
        _repo = repo;
        _assignmentRepo = assignmentRepo;
    }

    public async Task<IEnumerable<AssignmentSheet>> GetAllAssignmentSheets()
    {
        return await _repo.GetAllAsync();
    }

    public async Task<AssignmentSheet?> GetAssignmentSheet(Guid id)
    {
        return await _repo.GetByIdAsync(id);
    }

    public async Task<AssignmentSheet> CreateAssignmentSheet(AssignmentSheet sheet, IEnumerable<Guid>? assignmentIds = null)
    {
        var created = await _repo.CreateAsync(sheet);

        if (assignmentIds != null)
        {
            await _repo.SetAssignmentsAsync(created.Id, assignmentIds);
        }

        return created;
    }

    public async Task<bool> UpdateAssignmentSheet(AssignmentSheet sheet, IEnumerable<Guid>? assignmentIds = null)
    {
        var updated = await _repo.UpdateAsync(sheet);
        if (!updated)
        {
            return false;
        }

        if (assignmentIds != null)
        {
            await _repo.SetAssignmentsAsync(sheet.Id, assignmentIds);
        }

        return true;
    }

    public async Task<bool> DeleteAssignmentSheet(Guid id)
    {
        return await _repo.DeleteAsync(id);
    }

    public async Task<AssignmentSheetPointsDTO?> GetPointsBreakdown(Guid sheetId)
    {
        var sheet = await _repo.GetByIdAsync(sheetId);
        if (sheet == null)
        {
            return null;
        }

        var perAssignment = sheet.Assignments
            .Select(a => new AssignmentPointsDTO
            {
                AssignmentId = a.Id,
                Number = a.Number,
                Points = a.Points
            })
            .ToList();

        return new AssignmentSheetPointsDTO
        {
            AssignmentSheetId = sheet.Id,
            TotalPoints = perAssignment.Sum(p => p.Points),
            PerAssignment = perAssignment
        };
    }
}
