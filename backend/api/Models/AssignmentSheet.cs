using System.ComponentModel.DataAnnotations;

namespace api.Models;

public class AssignmentSheet
{
    [Key]
    public Guid Id { get; init; }
    public string Title { get; set; } = "";
    public string Subject { get; set; } = "";
    public string Level { get; set; } = "";
    public int Year { get; set; } = DateTime.Today.Year;
    public string Owner { get; set; } = "Prøvebank";
    public AssignmentSheetType Type { get; set; } = AssignmentSheetType.Hjemmeopgave;
    public List<Assignment> Assignments { get; set; } = new List<Assignment>();

    [Timestamp]
    public byte[]? RowVersion { get; set; }
}
