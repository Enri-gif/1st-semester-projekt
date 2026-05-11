using api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared;
using api.Interfaces;

namespace api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = Roles.Student)]
public class StudentEvaluationController : ControllerBase
{
    private const double CloseThresholdFraction = 0.8;

    private readonly IAssignmentService _assignmentService;

    public StudentEvaluationController(IAssignmentService assignmentService)
    {
        _assignmentService = assignmentService;
    }

    [HttpGet("{assignmentId}")]
    public async Task<ActionResult<string>> GetEvaluation(Guid assignmentId, [FromQuery] int studentPoints)
    {
        var assignment = await _assignmentService.GetById(assignmentId);
        if (assignment == null)
        {
            return NotFound();
        }

        if (assignment.Points <= 0)
        {
            return Forbid();
        }

        var threshold = assignment.Points * CloseThresholdFraction;
        if (studentPoints < threshold)
        {
            return StatusCode(StatusCodes.Status403Forbidden,
                new { message = "Du er ikke tæt nok på lærerens vurdering til at se den." });
        }

        return Ok(new { teacherEvaluation = assignment.TeacherEvaluation });
    }
}
