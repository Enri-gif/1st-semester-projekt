using api.Data;
using Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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
        ValidateAudience = false
    };
});

builder.Services.AddCors (options =>
{
    options.AddPolicy ("AllowWasmClient", policy =>
    {
        policy.WithOrigins ("https://localhost:7273")
              .AllowAnyMethod ()
              .AllowAnyHeader ();
    });
});

builder.Services.AddScoped<ITokenService, TokenService> ();

// Only required for when seeding below
//builder.Services.AddScoped<DbSeeder> ();

//builder.Services.AddRazorPages ();
builder.Services.AddServerSideBlazor ();

builder.Services.AddControllers ();

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

app.UseCors ("AllowWasmClient");

app.UseAuthentication ();
app.UseAuthorization ();

app.MapControllers ();

app.Run();
