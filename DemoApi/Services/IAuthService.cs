namespace DemoApi.Services;

public interface IAuthService : IAppService
{
    Task<string> Authenticate(string email, string password, string passwordHash);
    Task Logout(string authorizationHeader);
}