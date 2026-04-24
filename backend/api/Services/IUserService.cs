using Microsoft.AspNetCore.Identity;

namespace api.Services;

public interface IUserService
{
    Task<IdentityUser?> ValidateUser(string username, string password);
}
