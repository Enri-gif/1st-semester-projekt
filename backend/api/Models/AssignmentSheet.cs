namespace api.Models;

public class AssignmentSheet
{
    public int Id { get; set; }
    public List<Assignment> Tasks { get; set; } = new();
    public string Type { get; set; } = ""; // "prøve" eller "lektier"
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}