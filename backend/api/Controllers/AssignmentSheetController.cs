using Microsoft.AspNetCore.Mvc;
using api.Models;
using api.DTOs;
using api.Services;
using QuestPDF.Fluent;

namespace api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AssignmentSheetController : ControllerBase
{
    private readonly AssignmentService _assignmentService;

    public AssignmentSheetController(AssignmentService assignmentService)
    {
        _assignmentService = assignmentService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAssignmentSheetDto dto)
    {
        if (dto.TaskIds == null || !dto.TaskIds.Any())
            return BadRequest("Ingen opgaver valgt");

        if (dto.Type != "prøve" && dto.Type != "lektier")
            return BadRequest("Ugyldig type");

        var allAssignments = await _assignmentService.GetAll();

        var selectedTasks = allAssignments
            .Where(a => dto.TaskIds.Contains(a.Id))
            .ToList();

        var sheet = new AssignmentSheet
        {
            Tasks = selectedTasks,
            Type = dto.Type
        };

        var pdfBytes = GeneratePdf(sheet);

        return File(pdfBytes, "application/pdf", "opgavesaet.pdf");
    }

    private byte[] GeneratePdf(AssignmentSheet sheet)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(20);
                page.Content().Column(col =>
                {
                    col.Item().Text($"Opgavesæt ({sheet.Type})").FontSize(20).Bold();

                    foreach (var task in sheet.Tasks)
                    {
                        col.Item().Text($"{task.Subject} - {task.Topic}").Bold();
                        col.Item().Text($"Spørgsmål {task.Number}");
                        col.Item().Text($"Svar: {task.Answer}");
                        col.Item().Text($"Point: {task.Points}");
                        col.Item().PaddingBottom(10);
                    }
                });
            });
        }).GeneratePdf();
    }
}