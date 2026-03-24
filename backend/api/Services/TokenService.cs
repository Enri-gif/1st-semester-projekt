using api.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Api.Services;

public interface ITokenService
{
    Task<string> CreateToken(ApplicationUser user);
}

public class TokenService : ITokenService
{
    private readonly IConfiguration config;
    private readonly UserManager<ApplicationUser> userManager;

    public TokenService(IConfiguration config, UserManager<ApplicationUser> userManager)
    {
        this.config = config;
        this.userManager = userManager;
    }

    public async Task<string> CreateToken (ApplicationUser user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName!),
            new Claim(ClaimTypes.NameIdentifier, user.Id)
        };

        var roles = await userManager.GetRolesAsync(user);

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey (Encoding.UTF8.GetBytes (config["Jwt:Key"]!));
        var creds = new SigningCredentials (key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken (
            claims: claims,
            expires: DateTime.UtcNow.AddHours (1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler ().WriteToken (token);
    }
}
