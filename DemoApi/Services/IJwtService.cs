namespace DemoApi.Services;

public interface IJwtService : IAppService
{
    Task<string> GenerateJwtToken(string email);
}