namespace blazor.Services;

public interface IAuthService
{
    void SetToken (string token);
}

public class AuthService : IAuthService
{
    private readonly HttpClient http;

    public AuthService (HttpClient http)
    {
        this.http = http;
    }

    public void SetToken (string token)
    {
        http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue ("Bearer", token);
    }
}
