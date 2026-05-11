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

    // Sheet-level tag fields, mirroring Assignment so a sheet can carry its own
    // metadata independent of the assignments it contains. When a sheet field is
    // populated it OVERRIDES the corresponding aggregate computed from child
    // assignments (see AssignmentSheetExtensions.EffectiveSubject etc.).
    public string Topic { get; set; } = "";
    public string Education { get; set; } = "";
    public List<string> Tags { get; set; } = new List<string>();

    // Teacher correction context — populated when a sheet has been graded.
    public string Grade { get; set; } = "";
    public string Feedback { get; set; } = "";
    public string CorrectionNotes { get; set; } = "";

    public List<Assignment> Assignments { get; set; } = new List<Assignment>();

    [Timestamp]
    public byte[]? RowVersion { get; set; }
}
