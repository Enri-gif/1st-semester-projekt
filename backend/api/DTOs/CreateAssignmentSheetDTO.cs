namespace api.DTOs;

public class CreateAssignmentSheetDto
{
    public List<Guid> TaskIds { get; set; } = new(); // Kun ID'er er nødvendige for at oprette et AssignmentSheet
    public string Type { get; set; } = "";
}