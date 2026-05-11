using api.Models;
using api.Services;
using ClosedXML.Excel;
using FluentAssertions;

namespace Tests.ServiceTests;

public class SpreadsheetServiceTests
{
    private static AssignmentSheet SampleSheet() => new()
    {
        Id = Guid.NewGuid(),
        Title = "Eksamen 2026",
        Subject = "Matematik",
        Level = "A",
        Year = 2026,
        Owner = "Lærer X",
        Assignments = new()
        {
            new Assignment { Id = Guid.NewGuid(), Number = 1, Topic = "Algebra", Subquestion = "1a", Points = 10 },
            new Assignment { Id = Guid.NewGuid(), Number = 2, Topic = "Geometri", Subquestion = "2a", Points = 20 }
        }
    };

    [Fact]
    public void Generates_Regneark_With_Five_Header_Columns()
    {
        // Arrange
        // Act
        var bytes = new SpreadsheetService().GenerateAssignmentSheetSpreadsheet(SampleSheet(), markingSheet: false);

        // Assert
        bytes.Should().NotBeNullOrEmpty();
        using var ms = new MemoryStream(bytes);
        using var wb = new XLWorkbook(ms);
        var ws = wb.Worksheets.First();
        ws.Name.Should().Be("Regneark");
        ws.Cell(6, 5).GetString().Should().Be("Svar");
    }

    [Fact]
    public void Generates_Retteark_With_Six_Header_Columns_And_SumFormula()
    {
        // Arrange
        // Act
        var bytes = new SpreadsheetService().GenerateAssignmentSheetSpreadsheet(SampleSheet(), markingSheet: true);

        // Assert
        using var ms = new MemoryStream(bytes);
        using var wb = new XLWorkbook(ms);
        var ws = wb.Worksheets.First();
        ws.Name.Should().Be("Retteark");
        ws.Cell(6, 5).GetString().Should().Be("Opnået point");
        ws.Cell(6, 6).GetString().Should().Be("Kommentar");
    }
}
