using api.Controllers;
using api.DTOs;
using api.Models;
using Api.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Tests.TestData;

namespace Tests.ControllerTests;

public class StudentsControllerTests
{
    private readonly Mock<IStudentService> studentService;

    public StudentsControllerTests ()
    {
        studentService = new Mock<IStudentService>();
    }

    [Theory]
    [ClassData(typeof(CreateStudentDTOTestData))]
    public async Task CreateStudent_WithClassData_Succeeds (CreateStudentDTO student)
    {
        // Arrange
        var studentCon = new StudentsController(studentService.Object);
        var studentType = new Student() { FirstName = student.FirstName, LastName = student.LastName };
        studentService.Setup(s => s.AddStudent(studentType)).ReturnsAsync(true);

        // Act
        var controllerAddResult = await studentCon.CreateStudent(student);

        // Assert
        Assert.IsType<ActionResult<Student>>(controllerAddResult);
        Assert.Equal(student.FirstName, studentType.FirstName);
        Assert.Equal(student.LastName, studentType.LastName);
    }

    [Theory]
    [InlineData("Bob", "Smirnoff")]
    [InlineData("Thomas", "Iglesias")]
    [InlineData("Valorious", "Terpentine")]
    public async Task UpdateStudent_WhenStudentExists_Succeeds (string updatedFirstName, string updatedLastName)
    {
        // Arrange
        var studentCon = new StudentsController (studentService.Object);
        studentService.Setup (s => s.GetStudent (0)).ReturnsAsync (new Student { Id = 0, FirstName = "Bob", LastName = "Iglesias" });
        var updateStudent = new Student ()
        {
            Id = 0,
            FirstName = updatedFirstName,
            LastName = updatedLastName
        };

        // Act
        await studentCon.UpdateStudent (updateStudent.Id, updateStudent);
        var postupdateStudent = await studentCon.GetStudent (0);

        // Assert
        Assert.Equal (0, postupdateStudent.Value!.Id);
        Assert.Equal (updatedFirstName, postupdateStudent.Value!.FirstName);
        Assert.Equal (updatedLastName, postupdateStudent.Value!.LastName);
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
        Assert.IsType<NoContentResult> (controllerDeleteResult);
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
        Assert.IsType<BadRequestResult> (controllerDeleteResult);
        studentService.Verify (s => s.DeleteStudent (1), Times.Once);
    }

}
