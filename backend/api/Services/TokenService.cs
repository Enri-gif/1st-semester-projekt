using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Api.Services;

public interface ITokenService
{
    string CreateToken (IdentityUser user);
}

public class TokenService : ITokenService
{
    private readonly string key = "B4ll1st1skM1ss1l4ffyr3nd3V4ff3lh3st!"; // TODO: move to config

    public string CreateToken (IdentityUser user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.UserName!),
            new Claim(ClaimTypes.NameIdentifier, user.Id)
        };

        var key = new SymmetricSecurityKey (Encoding.UTF8.GetBytes (this.key));
        var creds = new SigningCredentials (key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken (
            claims: claims,
            expires: DateTime.UtcNow.AddHours (1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler ().WriteToken (token);
    }
}
