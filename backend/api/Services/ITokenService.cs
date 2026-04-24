using api.Data;

namespace api.Services;

public interface ITokenService
{
    Task<string> CreateToken(ApplicationUser user);
}
