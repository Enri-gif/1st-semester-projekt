using api.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using api.Services;
using Api.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("DevCors", policy =>
    {
        policy.AllowAnyHeader()
              .AllowAnyMethod()
              .WithOrigins("http://localhost:5001"); // frontend URL
    });
});

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

//hvorfor?
//builder.Services.AddRazorPages ();
builder.Services.AddServerSideBlazor ();

builder.Services.AddControllers ();
builder.Services.AddScoped<AssignmentService>();
builder.Services.AddScoped<IStudentService, StudentService> ();
builder.Services.AddSingleton<MongoAttachmentService>();

var app = builder.Build();
app.UseCors("DevCors");

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

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.UseCors ("AllowWasmClient");
app.MapControllers();

app.UseAuthentication ();
app.UseAuthorization ();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
