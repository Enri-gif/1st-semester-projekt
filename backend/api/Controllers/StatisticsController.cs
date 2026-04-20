using api.Data;
using api.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared;

namespace api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = Roles.StudentOrTeacher)]
public class StatisticsController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;

    public StatisticsController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet("summary")]
    public async Task<ActionResult<StatisticsDto>> GetSummary()
    {
        var assignments = await _dbContext.Assignments.AsNoTracking().ToListAsync();

        var bySubject = assignments
            .GroupBy(a => string.IsNullOrWhiteSpace(a.Subject) ? "(Ukendt)" : a.Subject)
            .Select(g => new SubjectStat
            {
                Subject = g.Key,
                Count = g.Count(),
                TotalPoints = g.Sum(a => a.Points),
                AveragePoints = g.Average(a => (double)a.Points)
            })
            .OrderByDescending(s => s.TotalPoints)
            .ToList();

        var byTopic = assignments
            .GroupBy(a => string.IsNullOrWhiteSpace(a.Topic) ? "(Ukendt)" : a.Topic)
            .Select(g => new TopicStat
            {
                Topic = g.Key,
                Count = g.Count(),
                TotalPoints = g.Sum(a => a.Points),
                AveragePoints = g.Average(a => (double)a.Points)
            })
            .OrderByDescending(s => s.TotalPoints)
            .ToList();

        return Ok(new StatisticsDto { BySubject = bySubject, ByTopic = byTopic });
    }
}
