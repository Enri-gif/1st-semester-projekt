using api.Data;
using Microsoft.AspNetCore.Identity;

namespace api.Interfaces;

public interface IUserService
{
    Task<ApplicationUser?> ValidateUser(string username, string password);
}
