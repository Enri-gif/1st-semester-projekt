namespace blazor.Interfaces;

public interface IAuthService
{
    void SetToken(string token);
    void SignOut();
}
