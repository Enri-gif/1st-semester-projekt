namespace api.DTOs;

public class AssignmentSheetPointsDto
{
    public Guid AssignmentSheetId { get; set; }
    public int TotalPoints { get; set; }
    public IEnumerable<AssignmentPointsDto> PerAssignment { get; set; } = Array.Empty<AssignmentPointsDto>();
}

public class AssignmentPointsDto
{
    public Guid AssignmentId { get; set; }
    public int Number { get; set; }
    public int Points { get; set; }
}
