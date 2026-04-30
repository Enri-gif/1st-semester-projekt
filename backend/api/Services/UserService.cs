using api.Data;
using Microsoft.AspNetCore.Identity;

namespace api.Services;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> userManager;

    public UserService(UserManager<ApplicationUser> userManager)
    {
        this.userManager = userManager;
    }

    public async Task<ApplicationUser?> ValidateUser(string username, string password)
    {
        var user = await userManager.FindByNameAsync(username);

        if (user == null)
        {
            return null;
        }

        var valid = await userManager.CheckPasswordAsync(user, password);

        return valid ? user : null;
    }
}
