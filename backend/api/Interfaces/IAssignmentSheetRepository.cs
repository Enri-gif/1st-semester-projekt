public interface IAssignmentSheetRepository
{
    Task<bool> DeleteAsync(int id);
}
