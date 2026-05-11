using api.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using api.Interfaces;

namespace api.Services;

public class AssignmentSheetPdfService : IAssignmentSheetPdfService
{
    static AssignmentSheetPdfService()
    {
        // QuestPDF community licence — free for individuals, students and
        // companies below the revenue threshold. This project qualifies.
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public byte[] GenerateAssignmentSheetPdf(AssignmentSheet sheet)
    {
        var typeLabel = sheet.Type.ToString();

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(t => t.FontSize(11));

                page.Header()
                    .Column(col =>
                    {
                        col.Item().Text(sheet.Title).FontSize(20).Bold();
                        col.Item().Text($"{typeLabel} · {sheet.Subject} · Niveau {sheet.Level} · {sheet.Year}")
                            .FontColor(Colors.Grey.Darken1);
                        col.Item().Text($"Ejer: {sheet.Owner}").FontColor(Colors.Grey.Darken1);
                    });

                page.Content()
                    .PaddingVertical(1, Unit.Centimetre)
                    .Column(col =>
                    {
                        col.Spacing(12);

                        if (sheet.Assignments.Count == 0)
                        {
                            col.Item().Text("(Ingen opgaver tilknyttet)").Italic();
                            return;
                        }

                        foreach (var a in sheet.Assignments.OrderBy(x => x.Number))
                        {
                            col.Item().Column(block =>
                            {
                                block.Item().Text($"Opgave {a.Number}  ·  {a.Points} p.")
                                    .Bold().FontSize(13);

                                if (!string.IsNullOrWhiteSpace(a.Topic))
                                    block.Item().Text($"Emne: {a.Topic}").FontColor(Colors.Grey.Darken1);

                                if (!string.IsNullOrWhiteSpace(a.Subquestion))
                                    block.Item().PaddingTop(4).Text(a.Subquestion);
                            });
                        }
                    });

                page.Footer()
                    .AlignCenter()
                    .Text(t =>
                    {
                        t.Span("Side ");
                        t.CurrentPageNumber();
                        t.Span(" af ");
                        t.TotalPages();
                    });
            });
        });

        return document.GeneratePdf();
    }
}
