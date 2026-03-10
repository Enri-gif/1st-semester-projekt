using Microsoft.AspNetCore.Identity;

namespace api.Data;

public class DbSeeder
{
    private readonly IServiceProvider serviceProvider;

    public DbSeeder (IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    public async Task SeedAsync ()
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>> ();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>> ();

        string[] roles = { "Admin", "Teacher", "Student" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync (role))
            {
                var roleResult = await roleManager.CreateAsync (new IdentityRole (role));
                if (!roleResult.Succeeded)
                    Console.WriteLine ($"Role creation failed: {role} -> {string.Join (", ", roleResult.Errors.Select (e => e.Description))}");
            }
        }

        var users = new[]
        {
        new { Email = "admin@example.com", Password = "Admin1234!", Role = "Admin" },
        new { Email = "teacher@teacher.com", Password = "Teacher1234!", Role = "Teacher" },
        new { Email = "student@student.com", Password = "Student1234!", Role = "Student" }
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
                    Console.WriteLine ($"User creation failed: {u.Email} -> {string.Join (", ", result.Errors.Select (e => e.Description))}");
                    continue;
                }
            }

            if (!await userManager.IsInRoleAsync (user, u.Role))
            {
                var roleResult = await userManager.AddToRoleAsync (user, u.Role);
                if (!roleResult.Succeeded)
                    Console.WriteLine ($"Role assignment failed: {u.Email} -> {string.Join (", ", roleResult.Errors.Select (e => e.Description))}");
                else
                    Console.WriteLine ($"Created {u.Email} with role {u.Role}");
            }
        }
    }
}
