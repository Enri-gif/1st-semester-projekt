namespace blazor.models;

public enum AssignmentSheetType
{
    Hjemmeopgave = 0,
    Proeve = 1
}

public class CreateAssignmentSheetDTO
{
    public string Title { get; set; } = "";
    public string Subject { get; set; } = "";
    public string Level { get; set; } = "";
    public int Year { get; set; } = System.DateTime.Today.Year;
    public string Owner { get; set; } = "Prøvebank";
    public AssignmentSheetType Type { get; set; } = AssignmentSheetType.Hjemmeopgave;
    public List<System.Guid> AssignmentIds { get; set; } = new();
}
