using Microsoft.AspNetCore.Components.Authorization;

namespace blazor.Services;

public interface IAuthService
{
    Task SetTokenAsync(string token);
    void SignOut();
}

public class AuthService : IAuthService
{
    private readonly HttpClient http;
    private readonly AuthenticationStateProvider authStateProvider;

    public AuthService(HttpClient http, AuthenticationStateProvider authStateProvider)
    {
        this.http = http;
        this.authStateProvider = authStateProvider;
    }

    public async Task SetTokenAsync(string token)
    {
        http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        if (authStateProvider is ApiAuthStateProvider api)
        {
            await api.NotifyUserAuthenticatedAsync();
        }
    }

    public void SignOut()
    {
        http.DefaultRequestHeaders.Authorization = null;

        if (authStateProvider is ApiAuthStateProvider api)
        {
            api.NotifyUserSignedOut();
        }
    }
}
