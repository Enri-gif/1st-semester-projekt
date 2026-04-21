namespace blazor.models;

public class AssignmentSheet
{
    public List<Assignment> Tasks { get; set; } = new();
    public string Type { get; set; } = "";
}