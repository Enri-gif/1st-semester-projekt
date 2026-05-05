using api.Data;
using api.DTOs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace api.Controllers;

[ApiController]
[Route("api/[controller]")]
[EnableCors("DevCors")]
public class AccountController : ControllerBase
{
    private readonly ILogger<AccountController> logger;
    private readonly UserManager<ApplicationUser> userManager;

    public AccountController(ILogger<AccountController> logger, UserManager<ApplicationUser> userManager)
    {
        this.logger = logger;
        this.userManager = userManager;
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

    [HttpPost ("change-password")]
    public async Task<IActionResult> ChangePassword ([FromBody] ChangePasswordDTO dto)
    {
        if (dto == null)
            return BadRequest ();

        var user = await userManager.GetUserAsync (User);
        if (user == null)
        {
            return Unauthorized ();
        }

        var result = await userManager.ChangePasswordAsync (
            user,
            dto.OldPassword,
            dto.NewPassword
        );

        user.HasChangedPasswordFirstTime = true;
        await userManager.UpdateAsync (user);

        if (!result.Succeeded)
        {
            return BadRequest (result.Errors.Select (e => e.Description));
        }

        return Ok ();
    }

    [HttpGet ("password-firstchanged-status")]
    public async Task<IActionResult> GetPasswordStatus ()
    {
        var user = await userManager.GetUserAsync (User);

        if (user == null)
        {
            return Unauthorized ();
        }

        return Ok (!user.HasChangedPasswordFirstTime);
    }

    [HttpPost ("logout")]
    [Authorize]
    public async Task<IActionResult> Logout ()
    {
        await HttpContext.SignOutAsync ();
        return Ok ();
    }

    [HttpGet("req-test")]
    public async Task<IActionResult> ReqTest ()
    {
        return Ok("Hello from ReqTest. Bloink!");
    }

}
