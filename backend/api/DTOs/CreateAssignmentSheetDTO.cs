using System.ComponentModel.DataAnnotations;
using api.Models;

namespace api.DTOs;

public class CreateAssignmentSheetDTO
{
    [Required]
    public string Title { get; set; } = "";
    public string Subject { get; set; } = "";
    public string Level { get; set; } = "";
    public int Year { get; set; } = DateTime.Today.Year;
    public string Owner { get; set; } = "Prøvebank";
    public AssignmentSheetType Type { get; set; } = AssignmentSheetType.Hjemmeopgave;

    // Optional: IDs of existing assignments to include in the new sheet.
    public List<Guid> AssignmentIds { get; set; } = new();
}
