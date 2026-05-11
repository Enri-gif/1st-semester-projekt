using blazor;
using blazor.Interfaces;
using blazor.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Authorization;
using Shared;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped<IAuthService, AuthService> ();

builder.Services.AddScoped<AuthenticationStateProvider, ApiAuthStateProvider>();

var apiBaseUrl = builder.Configuration["ApiBaseUrl"]
                 ?? throw new InvalidOperationException("ApiBaseUrl not configured in appsettings.json");
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(apiBaseUrl)
});

builder.Services.AddAuthorizationCore(options =>
{
    options.AddPolicy("StudentOrTeacher", policy =>
        policy.RequireRole(Roles.Student, Roles.Teacher));
});

await builder.Build().RunAsync();
