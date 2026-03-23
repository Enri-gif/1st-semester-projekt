using Microsoft.AspNetCore.Identity;

namespace Api.Services;

public interface IUserService
{
    Task<IdentityUser?> ValidateUser (string username, string password);
}

public class UserService : IUserService
{
    private readonly UserManager<IdentityUser> userManager;

    public UserService (UserManager<IdentityUser> userManager)
    {
        this.userManager = userManager;
    }

    public async Task<IdentityUser?> ValidateUser (string username, string password)
    {
        var user = await userManager.FindByNameAsync (username);

        if (user == null)
            return null;

        var valid = await userManager.CheckPasswordAsync (user, password);

        return valid ? user : null;
    }
}
