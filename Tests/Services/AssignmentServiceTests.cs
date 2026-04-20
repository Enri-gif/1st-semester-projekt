using api.Data;
using api.Models;
using api.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Tests.ServiceTests;

public class AssignmentServiceTests
{
    private static (AssignmentService svc, ApplicationDbContext ctx) Build()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var ctx = new ApplicationDbContext(options);
        return (new AssignmentService(ctx), ctx);
    }

    [Fact]
    public async Task GetFiltered_Clamps_PageSize_To_100()
    {
        var (svc, _) = Build();

        var result = await svc.GetFiltered(null, null, null, null, null, null, null, page: 1, pageSize: 9999, sortBy: null, sortDir: null);

        result.PageSize.Should().Be(100);
    }

    [Fact]
    public async Task GetFiltered_FiltersByTag()
    {
        var (svc, ctx) = Build();
        ctx.Assignments.AddRange(
            new Assignment { Id = Guid.NewGuid(), Subject = "M", Tags = new() { "calc", "easy" } },
            new Assignment { Id = Guid.NewGuid(), Subject = "M", Tags = new() { "geo" } }
        );
        await ctx.SaveChangesAsync();

        var result = await svc.GetFiltered(null, null, null, null, null, "calc", null, 1, 20, null, null);

        result.Total.Should().Be(1);
    }

    [Fact]
    public async Task GetFiltered_DefaultSort_IsByDate_Descending()
    {
        var (svc, ctx) = Build();
        var older = DateTime.Today.AddDays(-3);
        var newer = DateTime.Today;
        ctx.Assignments.AddRange(
            new Assignment { Id = Guid.NewGuid(), Subject = "M", Date = older, Number = 1 },
            new Assignment { Id = Guid.NewGuid(), Subject = "M", Date = newer, Number = 2 }
        );
        await ctx.SaveChangesAsync();

        var result = await svc.GetFiltered(null, null, null, null, null, null, null, 1, 20, null, "desc");

        result.Items.First().Date.Should().Be(newer);
    }
}
