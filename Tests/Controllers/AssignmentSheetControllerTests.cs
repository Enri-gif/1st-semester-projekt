using api.Controllers;
using api.Data;
using api.DTOs;
using api.Models;
using api.Services;
using api.Validators;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Tests.ControllerTests;

public class AssignmentSheetControllerTests
{
    private static AssignmentSheetController Build(Mock<IAssignmentSheetRepository> repo)
    {
        var service = new AssignmentSheetService(repo.Object);
        return new AssignmentSheetController(
            service,
            new SpreadsheetService(),
            new CreateAssignmentSheetDtoValidator(),
            new UpdateAssignmentSheetDtoValidator());
    }

    [Fact]
    public async Task CreateAssignmentSheet_AssignsServerSideId_AndReturnsCreated()
    {
        var repo = new Mock<IAssignmentSheetRepository>();
        repo.Setup(r => r.CreateAsync(It.IsAny<AssignmentSheet>())).ReturnsAsync((AssignmentSheet s) => s);
        var controller = Build(repo);

        var dto = new CreateAssignmentSheetDto { Title = "Test", Subject = "Math", Year = 2026 };
        var result = await controller.CreateAssignmentSheet(dto);

        var created = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        var sheet = created.Value.Should().BeOfType<AssignmentSheet>().Subject;
        sheet.Id.Should().NotBe(Guid.Empty);
        sheet.Title.Should().Be("Test");
    }

    [Fact]
    public async Task CreateAssignmentSheet_RejectsInvalidYear()
    {
        var repo = new Mock<IAssignmentSheetRepository>();
        var controller = Build(repo);

        var dto = new CreateAssignmentSheetDto { Title = "Test", Year = 1800 };
        var result = await controller.CreateAssignmentSheet(dto);

        result.Result.Should().BeAssignableTo<ObjectResult>()
            .Which.Value.Should().BeOfType<ValidationProblemDetails>();
        repo.Verify(r => r.CreateAsync(It.IsAny<AssignmentSheet>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAssignmentSheet_UsesRouteId_NotBodyId()
    {
        var routeId = Guid.NewGuid();
        AssignmentSheet? captured = null;
        var repo = new Mock<IAssignmentSheetRepository>();
        repo.Setup(r => r.UpdateAsync(It.IsAny<AssignmentSheet>()))
            .Callback<AssignmentSheet>(s => captured = s)
            .ReturnsAsync(true);

        var controller = Build(repo);

        var dto = new UpdateAssignmentSheetDto { Title = "Updated", Year = 2026 };
        var result = await controller.UpdateAssignmentSheet(routeId, dto);

        result.Should().BeOfType<NoContentResult>();
        captured!.Id.Should().Be(routeId);
    }

    [Fact]
    public async Task DeleteAssignmentSheet_ReturnsNotFound_WhenMissing()
    {
        var repo = new Mock<IAssignmentSheetRepository>();
        repo.Setup(r => r.DeleteAsync(It.IsAny<Guid>())).ReturnsAsync(false);

        var result = await Build(repo).DeleteAssignmentSheet(Guid.NewGuid());

        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GetSpreadsheet_ReturnsXlsxFile_WhenSheetExists()
    {
        var sheet = new AssignmentSheet
        {
            Id = Guid.NewGuid(),
            Title = "Eksamen",
            Assignments = new() { new Assignment { Id = Guid.NewGuid(), Number = 1, Points = 5 } }
        };
        var repo = new Mock<IAssignmentSheetRepository>();
        repo.Setup(r => r.GetByIdAsync(sheet.Id)).ReturnsAsync(sheet);

        var result = await Build(repo).GetSpreadsheet(sheet.Id, marking: false);

        var file = result.Should().BeOfType<FileContentResult>().Subject;
        file.ContentType.Should().Be("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        file.FileDownloadName.Should().EndWith("-regneark.xlsx");
    }
}
