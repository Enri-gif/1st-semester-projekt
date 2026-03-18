using api.Controllers;
using Api.Services;
using Moq;

namespace Tests.ControllerTests;

public class StudentsControllerTests
{
    private readonly Mock<IStudentService> studentService;

    public StudentsControllerTests ()
    {
        studentService = new Mock<IStudentService>();
    }

    [Fact]
    public async Task DeleteStudent_WhenStudentExists_Succeeds ()
    {
        // Arrange
        var studentsCon = new StudentsController (studentService.Object);
        studentService.Setup (s => s.DeleteStudent (1)).ReturnsAsync (true);

        // Act
        var controllerDeleteResult = await studentsCon.DeleteStudent (1);

        // Assert
        Assert.IsType<Microsoft.AspNetCore.Mvc.NoContentResult> (controllerDeleteResult);
        studentService.Verify (s => s.DeleteStudent (1), Times.Once);
    }

    [Fact]
    public async Task DeleteStudent_FailsWhen_StudentDoesntExist ()
    {
        // Arrange
        var studentsCon = new StudentsController (studentService.Object);
        studentService.Setup (s => s.DeleteStudent (1)).ReturnsAsync (false);

        // Act
        var controllerDeleteResult = await studentsCon.DeleteStudent (1);

        // Assert
        Assert.IsType<Microsoft.AspNetCore.Mvc.BadRequestResult> (controllerDeleteResult);
        studentService.Verify (s => s.DeleteStudent (1), Times.Once);
    }
}
