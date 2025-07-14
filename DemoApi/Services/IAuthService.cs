namespace DemoApi.Services;

public interface IAuthService
{
    Task<string> Authenticate(string email, string password, string passwordHash);
}