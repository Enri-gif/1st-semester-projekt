using api.Controllers;
using api.DTOs;
using api.Models;
using Api.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
//using Moq;
using Telerik.JustMock;

namespace Tests;

public class JustMock
{
    [Fact]
    public async Task CreateStudent_ReturnsCreated_WhenSuccessful()
    {
        // Arrange
        var studentService = Mock.Create<IStudentService>();

        Mock.Arrange(() => studentService.AddStudent(Arg.IsAny<Student>()))
            .Returns(Task.FromResult(true));

        var controller = new StudentsController(studentService);

        var dto = new CreateStudentDTO
        {
            FirstName = "John",
            LastName = "Doe"
        };

        // Act
        var result = await controller.CreateStudent(dto);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var student = Assert.IsType<Student>(createdResult.Value);

        Assert.Equal("John", student.FirstName);
        Assert.Equal("Doe", student.LastName);

        Mock.Assert(() => studentService.AddStudent(Arg.IsAny<Student>()), Occurs.Once());
    }
}
