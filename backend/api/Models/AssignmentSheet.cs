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

    // Teacher correction context — populated when a sheet has been graded.
    public string Grade { get; set; } = "";
    public string Feedback { get; set; } = "";
    public string CorrectionNotes { get; set; } = "";

    public List<Assignment> Assignments { get; set; } = new List<Assignment>();

    [Timestamp]
    public byte[]? RowVersion { get; set; }
}
