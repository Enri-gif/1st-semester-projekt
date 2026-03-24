public class AssignmentSheetService
{
    private readonly IAssignmentSheetRepository _repo;

    public AssignmentSheetService(IAssignmentSheetRepository repo)
    {
        _repo = repo;
    }

    public async Task<bool> DeleteAssignmentSheet(int id)
    {
        return await _repo.DeleteAsync(id);
    }
}