using api.Data;
using Api.DTOs;
using Api.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shared.Authorization;

namespace Api.Controllers;

[ApiController]
[Route ("api/auth")]
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
    public async Task<IActionResult> Login (/*[FromBody]*/ LoginModel model)
    {
        if (model == null)
            return BadRequest ("Login model is null");

        try
        {
            var user = await userManager.FindByNameAsync (model.UserName);

            var passwordHasher = new PasswordHasher<ApplicationUser> ();
            var verify = passwordHasher.VerifyHashedPassword (user, user.PasswordHash, "Password123!");
            Console.WriteLine ($"{verify} |||||| {user.PasswordHash}");

            if (user == null || !await userManager.CheckPasswordAsync (user, model.Password))
            {
                return Unauthorized ("This is a restricted area.");
            }

            var token = tokenService.CreateToken (user);
            return Ok (new LoginResult { Token = token });
        }
        catch (Exception ex)
        {
            // Log exception
            Console.WriteLine (ex);
            return StatusCode (500, ex.Message);
        }
    }
}
