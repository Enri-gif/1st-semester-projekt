using api.Data;
using Microsoft.AspNetCore.Identity;

namespace api.Services;

public interface IUserService
{
    Task<ApplicationUser?> ValidateUser(string username, string password);
}
