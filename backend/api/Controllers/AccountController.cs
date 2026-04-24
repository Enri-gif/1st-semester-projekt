using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace api.Controllers;

[ApiController]
[Route("api/[controller]")]
[EnableCors("DevCors")]
public class AccountController : ControllerBase
{
    private readonly ILogger<AccountController> logger;

    public AccountController(ILogger<AccountController> logger)
    {
        this.logger = logger;
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        var user = HttpContext.User;
        var name = user.Identity?.Name;

        var roles = user.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList();

        if (logger.IsEnabled(LogLevel.Debug))
        {
            foreach (var claim in user.Claims)
            {
                logger.LogDebug("{UserName} claim: {ClaimType}={ClaimValue}", name, claim.Type, claim.Value);
            }
        }

        return Ok(new
        {
            Name = name,
            Roles = roles
        });
    }
}
