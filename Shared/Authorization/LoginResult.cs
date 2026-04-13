namespace Shared.Authorization;

public class LoginResult
{
    public string Token { get; set; } = string.Empty;
    public string Role {  get; set; } = string.Empty;
}
