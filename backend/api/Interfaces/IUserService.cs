using Microsoft.AspNetCore.Identity;

namespace api.Interfaces;

public interface IUserService
{
    Task<IdentityUser?> ValidateUser(string username, string password);
}
