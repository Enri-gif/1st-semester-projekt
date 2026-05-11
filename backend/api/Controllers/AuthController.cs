using api.Data;
using api.DTOs;
using api.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shared.Authorization;
using api.Interfaces;

namespace api.Controllers;

[ApiController]
[Route ("api/auth")]
[EnableCors("DevCors")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> userManager;
    private readonly ITokenService tokenService;
    private readonly ILogger<AuthController> logger;

    public AuthController (UserManager<ApplicationUser> userManager, ITokenService tokenService, ILogger<AuthController> logger)
    {
        this.userManager = userManager;
        this.tokenService = tokenService;
        this.logger = logger;
    }

    [HttpPost ("login")]
    public async Task<IActionResult> Login ([FromBody] LoginModel model)
    {
        if (model == null)
        {
            logger.LogWarning("Login failed: body was null");
            return BadRequest ("Login model is null");
        }

        var user = await userManager.FindByNameAsync (model.UserName);

        if (user == null || !await userManager.CheckPasswordAsync (user, model.Password))
        {
            logger.LogInformation("Login failed for {UserName}", model.UserName);
            return Unauthorized ("This is a restricted area.");
        }

        var token = await tokenService.CreateToken (user);
        var roles = await userManager.GetRolesAsync (user);
        var firstRole = roles.FirstOrDefault() ?? "";

        logger.LogInformation("Login succeeded for {UserName} with role {Role}", user.UserName, firstRole);
        return Ok (new LoginResult { Token = token, Role = firstRole });
    }

}

