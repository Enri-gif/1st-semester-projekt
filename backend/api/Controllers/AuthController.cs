using api.Data;
using Api.DTOs;
using Api.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shared.Authorization;

namespace Api.Controllers;

[ApiController]
[Route ("api/auth")]
[EnableCors("DevCors")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> userManager;
    private readonly ITokenService tokenService;

    public AuthController (UserManager<ApplicationUser> userManager, ITokenService tokenService)
    {
        this.userManager = userManager;
        this.tokenService = tokenService;
    }

    [HttpPost ("login")]
    public async Task<IActionResult> Login ([FromBody] LoginModel model)
    {
        if (model == null)
        {
            Console.WriteLine ($"{0} - Bad Request: Login model is null.", "AuthController");
            return BadRequest ("Login model is null");
        }

        try
        {
            var user = await userManager.FindByNameAsync (model.UserName);

            if (user == null || !await userManager.CheckPasswordAsync (user, model.Password))
            {
                Console.WriteLine ($"{0} - Unauthorized: User null or password match failed for user.", "AuthController");
                return Unauthorized ("This is a restricted area.");
            }

            var token = await tokenService.CreateToken (user);
            var roles = await userManager.GetRolesAsync (user);
            var firstRole = roles.FirstOrDefault();
            
            Console.WriteLine ($"{0} - Succesful Login for {user}, {firstRole}", "AuthController");
            return Ok (new LoginResult { Token = token, Role = firstRole ?? "" });
        }
        catch (Exception ex)
        {
            Console.WriteLine ($"{0} - Exception thrown: {ex}.", "AuthController");
            return StatusCode (500, ex.Message);
        }
    }
}

