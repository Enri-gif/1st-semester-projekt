namespace blazor.Interfaces;

public interface IAuthService
{
    Task SetTokenAsync(string token);
    void SignOut();
}
