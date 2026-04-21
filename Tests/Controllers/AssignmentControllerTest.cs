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
        return new AssignmentController(service, mongoImageService: null!, new CreateAssignmentDtoValidator());
    }

    [Fact]
    public async Task GetAssignmentById_ShouldReturnAssignment_WhenExists()
    {
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

        var result = await controller.GetAssignmentById(assignment.Id);

        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeOfType<AssignmentResponseDto>()
            .Which.Id.Should().Be(assignment.Id);
    }

    [Fact]
    public async Task GetAssignmentById_ShouldReturnNotFound_WhenMissing()
    {
        var controller = CreateController(out _);

        var result = await controller.GetAssignmentById(Guid.NewGuid());

        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GetAssignments_ShouldFilterBySubject_AndPaginate()
    {
        var controller = CreateController(out var context);
        context.Assignments.AddRange(
            new Assignment { Id = Guid.NewGuid(), Subject = "Math", Points = 5,  Number = 1, Date = DateTime.Today.AddDays(-2) },
            new Assignment { Id = Guid.NewGuid(), Subject = "Math", Points = 10, Number = 2, Date = DateTime.Today.AddDays(-1) },
            new Assignment { Id = Guid.NewGuid(), Subject = "Danish", Points = 7, Number = 3, Date = DateTime.Today }
        );
        await context.SaveChangesAsync();

        var result = await controller.GetAssignments(
            subject: "Math",
            level: null, year: null, topic: null, owner: null, tag: null, assignmentSheetId: null,
            page: 1, pageSize: 10, sortBy: "points", sortDir: "desc");

        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var paged = ok.Value.Should().BeOfType<PagedResult<AssignmentResponseDto>>().Subject;
        paged.Total.Should().Be(2);
        paged.Items.Should().HaveCount(2);
        paged.Items.First().Points.Should().Be(10);
    }
}
