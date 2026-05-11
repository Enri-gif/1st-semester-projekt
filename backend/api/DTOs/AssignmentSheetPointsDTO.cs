namespace api.DTOs;

public class AssignmentSheetPointsDTO
{
    public Guid AssignmentSheetId { get; set; }
    public int TotalPoints { get; set; }
    public IEnumerable<AssignmentPointsDTO> PerAssignment { get; set; } = Array.Empty<AssignmentPointsDTO>();
}
