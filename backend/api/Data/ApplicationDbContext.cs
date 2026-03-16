using api.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using api.Models;

namespace api.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext (DbContextOptions<ApplicationDbContext> options)
            : base (options)
    {

    }

    public DbSet<Student> Students { get; set; }
    public DbSet<Assignment> Assignments { get; set; }
}
