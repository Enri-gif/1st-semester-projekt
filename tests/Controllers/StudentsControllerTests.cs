using api.Controllers;
using api.DTOs;
using api.Models;
using api.Services;
using api.Validators;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Tests.TestData;

namespace Tests.ControllerTests;

public class StudentsControllerTests
{
    private readonly Mock<IStudentService> studentService;

    public StudentsControllerTests()
    {
        studentService = new Mock<IStudentService>();
    }

    private StudentsController CreateController()
        => new StudentsController(studentService.Object, new CreateStudentDTOValidator(), NullLogger<StudentsController>.Instance);

    [Theory]
    [ClassData(typeof(CreateStudentDTOTestData))]
    public async Task CreateStudent_WithClassData_Succeeds(CreateStudentDTO student)
    {
        var studentCon = CreateController();
        studentService
            .Setup(s => s.AddStudent(It.Is<Student>(st => st.FirstName == student.FirstName && st.LastName == student.LastName)))
            .ReturnsAsync(true);

        var result = await studentCon.CreateStudent(student);

        result.Should().BeOfType<ActionResult<Student>>();
        var created = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        var saved = created.Value.Should().BeOfType<Student>().Subject;
        saved.FirstName.Should().Be(student.FirstName);
        saved.LastName.Should().Be(student.LastName);
    }

    [Fact]
    public async Task DeleteStudent_WhenStudentExists_Succeeds()
    {
        var studentsCon = CreateController();
        var id = Guid.NewGuid();
        studentService.Setup(s => s.DeleteStudent(id)).ReturnsAsync(true);

        var result = await studentsCon.DeleteStudent(id);

        result.Should().BeOfType<NoContentResult>();
        studentService.Verify(s => s.DeleteStudent(id), Times.Once);
    }

    [Fact]
    public async Task DeleteStudent_ReturnsNotFound_WhenStudentDoesntExist()
    {
        var studentsCon = CreateController();
        var id = Guid.NewGuid();
        studentService.Setup(s => s.DeleteStudent(id)).ReturnsAsync(false);

        var result = await studentsCon.DeleteStudent(id);

        result.Should().BeOfType<NotFoundResult>();
        studentService.Verify(s => s.DeleteStudent(id), Times.Once);
    }
}
