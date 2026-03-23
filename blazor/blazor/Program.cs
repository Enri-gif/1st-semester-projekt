using blazor;
using blazor.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7042/"/*builder.HostEnvironment.BaseAddress*/) });
builder.Services.AddScoped<IAuthService, AuthService> ();

await builder.Build().RunAsync();
