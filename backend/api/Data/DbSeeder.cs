using Microsoft.AspNetCore.Identity;
using Shared;

namespace api.Data;

public class DbSeeder
{
    private readonly IServiceProvider serviceProvider;
    private readonly ILogger<DbSeeder> logger;

    public DbSeeder (IServiceProvider serviceProvider, ILogger<DbSeeder> logger)
    {
        this.serviceProvider = serviceProvider;
        this.logger = logger;
    }

    public async Task SeedAsync ()
    {
        using var scope = serviceProvider.CreateScope ();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>> ();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>> ();

        string[] roles = { Roles.Admin, Roles.Teacher, Roles.Student };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync (role))
            {
                var roleResult = await roleManager.CreateAsync (new IdentityRole (role));
                if (!roleResult.Succeeded)
                    logger.LogError("Role creation failed: {Role} -> {Errors}", role, string.Join(", ", roleResult.Errors.Select(e => e.Description)));
            }
        }

        var users = new[]
        {
            new { Email = "admin@example.com", Password = "Admin1234!", Role = Roles.Admin },
            new { Email = "teacher@teacher.com", Password = "Teacher1234!", Role = Roles.Teacher },
            new { Email = "student@student.com", Password = "Student1234!", Role = Roles.Student }
        };

        foreach (var u in users)
        {
            var user = await userManager.FindByEmailAsync (u.Email);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = u.Email,
                    Email = u.Email,
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync (user, u.Password);
                if (!result.Succeeded)
                {
                    logger.LogError("User creation failed: {Email} -> {Errors}", u.Email, string.Join(", ", result.Errors.Select(e => e.Description)));
                    continue;
                }
            }

            if (!await userManager.IsInRoleAsync (user, u.Role))
            {
                var roleResult = await userManager.AddToRoleAsync (user, u.Role);
                if (!roleResult.Succeeded)
                {
                    logger.LogError("Role assignment failed: {Email} -> {Errors}", u.Email, string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                }
                else
                {
                    logger.LogInformation("Created {Email} with role {Role}", u.Email, u.Role);
                }
            }
        }
    }
}
