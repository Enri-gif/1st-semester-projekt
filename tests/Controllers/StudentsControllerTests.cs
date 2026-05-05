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
using api.Interfaces;

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
        // Arrange
        var studentCon = CreateController();
        studentService
            .Setup(s => s.AddStudent(It.Is<Student>(st => st.FirstName == student.FirstName && st.LastName == student.LastName)))
            .ReturnsAsync(true);

        // Act
        var result = await studentCon.CreateStudent(student);

        // Assert
        result.Should().BeOfType<ActionResult<StudentResponseDTO>>();
        var created = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        var saved = created.Value.Should().BeOfType<StudentResponseDTO>().Subject;
        saved.FirstName.Should().Be(student.FirstName);
        saved.LastName.Should().Be(student.LastName);
    }

    [Theory]
    [InlineData ("Bob", "Smirnoff")]
    [InlineData ("Thomas", "Iglesias")]
    [InlineData ("Valorious", "Terpentine")]
    public async Task UpdateStudent_WhenStudentExists_Succeeds (string updatedFirstName, string updatedLastName)
    {
        // Arrange
        var studentService = new Mock<IStudentService> ();
        var studentId = Guid.NewGuid();

        var storedStudent = new Student
        {
            Id = studentId,
            FirstName = "Bob",
            LastName = "Iglesias"
        };

        studentService
            .Setup (s => s.GetStudent (studentId))
            .ReturnsAsync (() => storedStudent);

        studentService
            .Setup (s => s.UpdateStudent (It.IsAny<Student> ()))
            .Callback<Student> (s =>
            {
                storedStudent.FirstName = s.FirstName;
                storedStudent.LastName = s.LastName;
            })
            .ReturnsAsync (true);

        var controller = new StudentsController (studentService.Object, new CreateStudentDTOValidator(), NullLogger<StudentsController>.Instance);

        var updateStudent = new Student
        {
            Id = studentId,
            FirstName = updatedFirstName,
            LastName = updatedLastName
        };

        // Act
        await controller.UpdateStudent (updateStudent);
        var result = await controller.GetStudent (studentId);

        // Assert
        Assert.NotNull (result.Value);
        Assert.Equal (studentId, result.Value.Id);
        Assert.Equal (updatedFirstName, result.Value.FirstName);
        Assert.Equal (updatedLastName, result.Value.LastName);
    }

    [Fact]
    public async Task DeleteStudent_WhenStudentExists_Succeeds()
    {
        // Arrange
        var studentsCon = CreateController();
        var id = Guid.NewGuid();
        studentService.Setup(s => s.DeleteStudent(id)).ReturnsAsync(true);

        // Act
        var result = await studentsCon.DeleteStudent(id);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        studentService.Verify(s => s.DeleteStudent(id), Times.Once);
    }

    [Fact]
    public async Task DeleteStudent_ReturnsNotFound_WhenStudentDoesntExist()
    {
        // Arrange
        var studentsCon = CreateController();
        var id = Guid.NewGuid();
        studentService.Setup(s => s.DeleteStudent(id)).ReturnsAsync(false);

        // Act
        var result = await studentsCon.DeleteStudent(id);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
        studentService.Verify(s => s.DeleteStudent(id), Times.Once);
    }
}
