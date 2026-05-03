using api.Data;
using api.Services;
using api.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using api.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddValidatorsFromAssemblyContaining<CreateStudentDTOValidator>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrWhiteSpace(jwtKey) || jwtKey.Length < 32)
{
    throw new InvalidOperationException("JWT key is missing or too short (min 32 chars).");
}

var validateIssuer = builder.Configuration.GetValue("Jwt:ValidateIssuer", false);
var validateAudience = builder.Configuration.GetValue("Jwt:ValidateAudience", false);
var validIssuer = builder.Configuration["Jwt:Issuer"];
var validAudience = builder.Configuration["Jwt:Audience"];
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = key,
        ValidateIssuer = validateIssuer,
        ValidateAudience = validateAudience,
        ValidIssuer = validIssuer,
        ValidAudience = validAudience,
        RoleClaimType = ClaimTypes.Role
    };
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("DevCors", policy =>
    {
        policy.WithOrigins(
                "http://localhost:5050",
                "https://localhost:5051")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<DbSeeder>();
builder.Services.AddServerSideBlazor();
builder.Services.AddControllers();
builder.Services.AddScoped<IAssignmentRepository, AssignmentRepository>();
builder.Services.AddScoped<IAssignmentService, AssignmentService>();
builder.Services.AddScoped<IAssignmentSheetRepository, AssignmentSheetRepository>();
builder.Services.AddScoped<IAssignmentSheetService, AssignmentSheetService>();
builder.Services.AddSingleton<SpreadsheetService>();
builder.Services.AddSingleton<IAssignmentSheetPdfService, AssignmentSheetPdfService>();
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddSingleton<MongoImageService>();
builder.Services.AddSingleton<MongoVideoService>();

var app = builder.Build();

if (builder.Configuration.GetValue("SeedDatabase", false))
{
    using var scope = app.Services.CreateScope();
    var seeder = scope.ServiceProvider.GetRequiredService<DbSeeder>();
    var startupLogger = scope.ServiceProvider.GetRequiredService<ILogger<DbSeeder>>();
    try
    {
        await seeder.SeedAsync();
    }
    catch (Exception ex)
    {
        // Don't hard-fail the API boot if the DB is unavailable (e.g. docker
        // not started yet). Log and continue; endpoints that need the DB
        // will surface a clearer error per request.
        startupLogger.LogError(ex, "Database seeding failed; continuing startup without seed data.");
    }
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var feature = context.Features.Get<IExceptionHandlerFeature>();
        var logger = context.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger("GlobalExceptionHandler");
        logger.LogError(feature?.Error, "Unhandled exception for {Path}", context.Request.Path);

        var problem = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "An unexpected error occurred.",
            Detail = app.Environment.IsDevelopment() ? feature?.Error.Message : null,
            Instance = context.Request.Path
        };

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/problem+json";
        await context.Response.WriteAsJsonAsync(problem);
    });
});

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("DevCors");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
