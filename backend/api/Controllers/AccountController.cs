using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using api.Data;
using Microsoft.AspNetCore.Cors;

[ApiController]
[Route("api/[controller]")]
[EnableCors("DevCors")]
public class AccountController : ControllerBase
{
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

        foreach (var claim in user.Claims)
        {
            Console.WriteLine($"{user.Identity?.Name} ||||||| {claim}");
        }

        return Ok(new
        {
            Name = name,
            Roles = roles
        });
    }

    [HttpPost("login-test")]
    public async Task<IActionResult> LoginTest([FromServices] SignInManager<ApplicationUser> signInManager)
    {
        var user = await signInManager.UserManager.FindByNameAsync("teacher");

        if (user == null)
        {
            return NotFound("User not found");
        }

        await signInManager.SignInAsync(user, isPersistent: true);

            return Ok();
    }
}