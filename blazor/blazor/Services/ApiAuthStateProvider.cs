using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Json;
using System.Security.Claims;

public class ApiAuthStateProvider : AuthenticationStateProvider
{
    private static readonly AuthenticationState Anonymous =
        new(new ClaimsPrincipal(new ClaimsIdentity()));

    private readonly HttpClient _http;
    private AuthenticationState? _cached;

    public ApiAuthStateProvider(HttpClient http)
    {
        _http = http;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if (_cached != null)
        {
            return _cached;
        }

        try
        {
            var userInfo = await _http.GetFromJsonAsync<UserInfo>("api/account/me");

            if (userInfo == null)
            {
                _cached = Anonymous;
                return _cached;
            }

            var claims = new List<Claim> { new Claim(ClaimTypes.Name, userInfo.Name) };
            claims.AddRange(userInfo.Roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var identity = new ClaimsIdentity(claims, "api");
            _cached = new AuthenticationState(new ClaimsPrincipal(identity));
            return _cached;
        }
        catch
        {
            _cached = Anonymous;
            return _cached;
        }
    }

    public void NotifyUserAuthenticated()
    {
        _cached = null;
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public void NotifyUserSignedOut()
    {
        _cached = Anonymous;
        NotifyAuthenticationStateChanged(Task.FromResult(Anonymous));
    }
}
