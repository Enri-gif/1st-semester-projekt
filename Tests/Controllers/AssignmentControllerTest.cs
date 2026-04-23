using api.Controllers;
using api.Data;
using api.DTOs;
using api.Models;
using api.Services;
using api.Validators;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Tests;

public class AssignmentControllerTests
{
    private static AssignmentController CreateController(out ApplicationDbContext context)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        context = new ApplicationDbContext(options);
        var repo = new AssignmentRepository(context);
        var service = new AssignmentService(repo, mongoVideoService: null!, mongoImageService: null!);
        return new AssignmentController(service, mongoImageService: null!, new CreateAssignmentDTOValidator());
    }

    [Fact]
    public async Task GetAssignmentById_ShouldReturnAssignment_WhenExists()
    {
        // Arrange
        var controller = CreateController(out var context);
        var assignment = new Assignment
        {
            Id = Guid.NewGuid(),
            Subject = "Math",
            Level = "A",
            Topic = "Algebra",
            Points = 10,
            Number = 1
        };
        context.Assignments.Add(assignment);
        await context.SaveChangesAsync();

        // Act
        var result = await controller.GetAssignmentById(assignment.Id);

        // Assert
        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeOfType<AssignmentResponseDTO>()
            .Which.Id.Should().Be(assignment.Id);
    }

    [Fact]
    public async Task GetAssignmentById_ShouldReturnNotFound_WhenMissing()
    {
        // Arrange
        var controller = CreateController(out _);

        // Act
        var result = await controller.GetAssignmentById(Guid.NewGuid());

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    // Classic xUnit-style variant kept deliberately so both assertion styles
    // are represented in the test suite.
    [Fact]
    public async Task GetAssignmentById_ShouldReturnNotFound_WhenMissing_ClassicXunit()
    {
        // Arrange
        var controller = CreateController(out _);

        // Act
        var result = await controller.GetAssignmentById(Guid.NewGuid());

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetAssignments_ShouldFilterBySubject_AndPaginate()
    {
        // Arrange
        var controller = CreateController(out var context);
        context.Assignments.AddRange(
            new Assignment { Id = Guid.NewGuid(), Subject = "Math", Points = 5,  Number = 1, Date = DateTime.Today.AddDays(-2) },
            new Assignment { Id = Guid.NewGuid(), Subject = "Math", Points = 10, Number = 2, Date = DateTime.Today.AddDays(-1) },
            new Assignment { Id = Guid.NewGuid(), Subject = "Danish", Points = 7, Number = 3, Date = DateTime.Today }
        );
        await context.SaveChangesAsync();

        // Act
        var result = await controller.GetAssignments(
            subject: "Math",
            level: null, year: null, topic: null, owner: null, tag: null, assignmentSheetId: null,
            page: 1, pageSize: 10, sortBy: "points", sortDir: "desc");

        // Assert
        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var paged = ok.Value.Should().BeOfType<PagedResult<AssignmentResponseDTO>>().Subject;
        paged.Total.Should().Be(2);
        paged.Items.Should().HaveCount(2);
        paged.Items.First().Points.Should().Be(10);
    }
}
