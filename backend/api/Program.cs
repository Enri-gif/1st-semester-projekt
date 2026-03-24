using api.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using api.Services;
using Api.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<ApplicationDbContext> (options =>
    options.UseSqlServer (builder.Configuration.GetConnectionString ("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole> (options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<ApplicationDbContext> ()
.AddDefaultTokenProviders ();

var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrWhiteSpace (jwtKey) || jwtKey.Length < 32)
{
    throw new InvalidOperationException ("JWT key is missing or too short (min 32 chars).");
}

var key = new SymmetricSecurityKey (Encoding.UTF8.GetBytes (jwtKey));

builder.Services.AddAuthentication (options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer (options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = key,
        ValidateIssuer = false,
        ValidateAudience = false,
        RoleClaimType = ClaimTypes.Role
    };
});

builder.Services.AddCors (options =>
{
    options.AddPolicy("DevCors", policy =>
    {
        policy.WithOrigins("https://localhost:5001", "http://localhost:5001") // frontend URL
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddScoped<ITokenService, TokenService> ();

// Only required for when seeding below
//builder.Services.AddScoped<DbSeeder> ();

//hvorfor?
//builder.Services.AddRazorPages ();
builder.Services.AddServerSideBlazor ();

builder.Services.AddControllers ();
builder.Services.AddScoped<AssignmentService>();
builder.Services.AddScoped<IStudentService, StudentService> ();
builder.Services.AddSingleton<MongoImageService>();
builder.Services.AddSingleton<MongoVideoService>();

var app = builder.Build();

// Enable to seed from DbSeeder-class
//using (var scope = app.Services.CreateScope ())
//{
//    var seeder = scope.ServiceProvider.GetRequiredService<DbSeeder> ();
//    await seeder.SeedAsync ();
//}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("DevCors");

app.UseAuthentication ();
app.UseAuthorization ();

app.MapControllers();

app.Run();
