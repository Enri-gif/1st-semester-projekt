using api.Data;

namespace api.Interfaces;

public interface ITokenService
{
    Task<string> CreateToken(ApplicationUser user);
}
