using System.ComponentModel.DataAnnotations;
using api.Models;

namespace api.DTOs;

public class UpdateAssignmentSheetDTO
{
    [Required]
    public string Title { get; set; } = "";
    public string Subject { get; set; } = "";
    public string Level { get; set; } = "";
    public int Year { get; set; } = DateTime.Today.Year;
    public string Owner { get; set; } = "Prøvebank";
    public AssignmentSheetType Type { get; set; } = AssignmentSheetType.Hjemmeopgave;

    public string Grade { get; set; } = "";
    public string Feedback { get; set; } = "";
    public string CorrectionNotes { get; set; } = "";

    // Replace the set of assignments attached to this sheet. If null, assignments are left untouched.
    public List<Guid>? AssignmentIds { get; set; }
}
