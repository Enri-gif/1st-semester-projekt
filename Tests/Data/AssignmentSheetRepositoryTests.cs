using api.Data;
using api.Models;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Tests.DataTests;

public class AssignmentSheetRepositoryTests
{
    private static (AssignmentSheetRepository repo, ApplicationDbContext ctx) Build()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var ctx = new ApplicationDbContext(options);
        return (new AssignmentSheetRepository(ctx), ctx);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsFalse_WhenSheetMissing()
    {
        var (repo, _) = Build();

        var result = await repo.DeleteAsync(Guid.NewGuid());

        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_RemovesSheet_AndReturnsTrue()
    {
        var (repo, ctx) = Build();
        var sheet = new AssignmentSheet { Id = Guid.NewGuid(), Title = "Test" };
        ctx.AssignmentSheets.Add(sheet);
        await ctx.SaveChangesAsync();

        var result = await repo.DeleteAsync(sheet.Id);

        result.Should().BeTrue();
        (await ctx.AssignmentSheets.FindAsync(sheet.Id)).Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_LeavesAssignmentsIntact_AndSetsFkToNull()
    {
        var (repo, ctx) = Build();
        var sheetId = Guid.NewGuid();
        var assignmentId = Guid.NewGuid();

        ctx.AssignmentSheets.Add(new AssignmentSheet { Id = sheetId, Title = "Parent" });
        ctx.Assignments.Add(new Assignment
        {
            Id = assignmentId,
            Subject = "Math",
            AssignmentSheetId = sheetId
        });
        await ctx.SaveChangesAsync();

        var deleted = await repo.DeleteAsync(sheetId);

        deleted.Should().BeTrue();
        var surviving = await ctx.Assignments.FindAsync(assignmentId);
        surviving.Should().NotBeNull();
        surviving!.AssignmentSheetId.Should().BeNull();
    }
}
