using api.Models;
using ClosedXML.Excel;

namespace api.Services;

public class SpreadsheetService
{
    public byte[] GenerateAssignmentSheetSpreadsheet(AssignmentSheet sheet, bool markingSheet)
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add(markingSheet ? "Retteark" : "Regneark");

        ws.Cell(1, 1).Value = "Opgavesæt:";
        ws.Cell(1, 2).Value = sheet.Title;
        ws.Cell(2, 1).Value = "Fag:";
        ws.Cell(2, 2).Value = sheet.Subject;
        ws.Cell(3, 1).Value = "Niveau:";
        ws.Cell(3, 2).Value = sheet.Level;
        ws.Cell(4, 1).Value = "År:";
        ws.Cell(4, 2).Value = sheet.Year;

        ws.Range(1, 1, 4, 1).Style.Font.Bold = true;

        int headerRow = 6;
        ws.Cell(headerRow, 1).Value = "Nr";
        ws.Cell(headerRow, 2).Value = "Emne";
        ws.Cell(headerRow, 3).Value = "Underspørgsmål";
        ws.Cell(headerRow, 4).Value = "Max point";

        if (markingSheet)
        {
            ws.Cell(headerRow, 5).Value = "Opnået point";
            ws.Cell(headerRow, 6).Value = "Kommentar";
        }
        else
        {
            ws.Cell(headerRow, 5).Value = "Svar";
        }

        ws.Range(headerRow, 1, headerRow, markingSheet ? 6 : 5).Style.Font.Bold = true;
        ws.Range(headerRow, 1, headerRow, markingSheet ? 6 : 5).Style.Fill.BackgroundColor = XLColor.LightGray;

        int row = headerRow + 1;
        foreach (var a in sheet.Assignments.OrderBy(a => a.Number))
        {
            ws.Cell(row, 1).Value = a.Number;
            ws.Cell(row, 2).Value = a.Topic;
            ws.Cell(row, 3).Value = a.Subquestion;
            ws.Cell(row, 4).Value = a.Points;

            if (!markingSheet)
            {
                ws.Cell(row, 5).Value = "";
            }

            row++;
        }

        int totalRow = row + 1;
        ws.Cell(totalRow, 3).Value = "I alt:";
        ws.Cell(totalRow, 3).Style.Font.Bold = true;
        ws.Cell(totalRow, 4).FormulaA1 = $"SUM(D{headerRow + 1}:D{row - 1})";
        ws.Cell(totalRow, 4).Style.Font.Bold = true;
        if (markingSheet)
        {
            ws.Cell(totalRow, 5).FormulaA1 = $"SUM(E{headerRow + 1}:E{row - 1})";
            ws.Cell(totalRow, 5).Style.Font.Bold = true;
        }

        ws.Columns().AdjustToContents();

        using var ms = new MemoryStream();
        workbook.SaveAs(ms);
        return ms.ToArray();
    }
}
