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
    private static AssignmentSheetController Build(Mock<IAssignmentSheetRepository> repo, Mock<IStudentService>? studentService = null)
    {
        var service = new AssignmentSheetService(repo.Object, Mock.Of<IAssignmentRepository>());
        return new AssignmentSheetController(
            service,
            new SpreadsheetService(),
            new AssignmentSheetPdfService(),
            (studentService ?? new Mock<IStudentService>()).Object,
            new CreateAssignmentSheetDTOValidator(),
            new UpdateAssignmentSheetDTOValidator());
    }

    [Fact]
    public async Task CreateAssignmentSheet_AssignsServerSideId_AndReturnsCreated()
    {
        // Arrange
        var repo = new Mock<IAssignmentSheetRepository>();
        repo.Setup(r => r.CreateAsync(It.IsAny<AssignmentSheet>())).ReturnsAsync((AssignmentSheet s) => s);
        var controller = Build(repo);
        var dto = new CreateAssignmentSheetDTO { Title = "Test", Subject = "Math", Year = 2026 };

        // Act
        var result = await controller.CreateAssignmentSheet(dto);

        // Assert
        var created = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        var sheet = created.Value.Should().BeOfType<AssignmentSheetResponseDTO>().Subject;
        sheet.Id.Should().NotBe(Guid.Empty);
        sheet.Title.Should().Be("Test");
    }

    [Fact]
    public async Task CreateAssignmentSheet_RejectsInvalidYear()
    {
        // Arrange
        var repo = new Mock<IAssignmentSheetRepository>();
        var controller = Build(repo);
        var dto = new CreateAssignmentSheetDTO { Title = "Test", Year = 1800 };

        // Act
        var result = await controller.CreateAssignmentSheet(dto);

        // Assert
        result.Result.Should().BeAssignableTo<ObjectResult>()
            .Which.Value.Should().BeOfType<ValidationProblemDetails>();
        repo.Verify(r => r.CreateAsync(It.IsAny<AssignmentSheet>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAssignmentSheet_UsesRouteId_NotBodyId()
    {
        // Arrange
        var routeId = Guid.NewGuid();
        AssignmentSheet? captured = null;
        var repo = new Mock<IAssignmentSheetRepository>();
        repo.Setup(r => r.UpdateAsync(It.IsAny<AssignmentSheet>()))
            .Callback<AssignmentSheet>(s => captured = s)
            .ReturnsAsync(true);
        var controller = Build(repo);
        var dto = new UpdateAssignmentSheetDTO { Title = "Updated", Year = 2026 };

        // Act
        var result = await controller.UpdateAssignmentSheet(routeId, dto);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        captured!.Id.Should().Be(routeId);
    }

    [Fact]
    public async Task DeleteAssignmentSheet_ReturnsNotFound_WhenMissing()
    {
        // Arrange
        var repo = new Mock<IAssignmentSheetRepository>();
        repo.Setup(r => r.DeleteAsync(It.IsAny<Guid>())).ReturnsAsync(false);

        // Act
        var result = await Build(repo).DeleteAssignmentSheet(Guid.NewGuid());

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task DeleteAssignmentSheet_ReturnsNoContent_OnSuccess()
    {
        // Arrange
        var id = Guid.NewGuid();
        var repo = new Mock<IAssignmentSheetRepository>();
        repo.Setup(r => r.DeleteAsync(id)).ReturnsAsync(true);

        // Act
        var result = await Build(repo).DeleteAssignmentSheet(id);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        repo.Verify(r => r.DeleteAsync(id), Times.Once);
    }

    // Classic xUnit-style variant (no FluentAssertions) kept deliberately
    // so both assertion styles are represented in the test suite.
    [Fact]
    public async Task DeleteAssignmentSheet_ReturnsNoContent_OnSuccess_ClassicXunit()
    {
        // Arrange
        var id = Guid.NewGuid();
        var repo = new Mock<IAssignmentSheetRepository>();
        repo.Setup(r => r.DeleteAsync(id)).ReturnsAsync(true);

        // Act
        var result = await Build(repo).DeleteAssignmentSheet(id);

        // Assert
        Assert.IsType<NoContentResult>(result);
        repo.Verify(r => r.DeleteAsync(id), Times.Once);
    }

    [Fact]
    public async Task GetSpreadsheet_ReturnsXlsxFile_WhenSheetExists()
    {
        // Arrange
        var sheet = new AssignmentSheet
        {
            Id = Guid.NewGuid(),
            Title = "Eksamen",
            Assignments = new() { new Assignment { Id = Guid.NewGuid(), Number = 1, Points = 5 } }
        };
        var repo = new Mock<IAssignmentSheetRepository>();
        repo.Setup(r => r.GetByIdAsync(sheet.Id)).ReturnsAsync(sheet);

        // Act
        var result = await Build(repo).GetSpreadsheet(sheet.Id, marking: false);

        // Assert
        var file = result.Should().BeOfType<FileContentResult>().Subject;
        file.ContentType.Should().Be("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        file.FileDownloadName.Should().EndWith("-regneark.xlsx");
    }
}
