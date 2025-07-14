namespace DemoApi.Services;

public interface IJwtService
{
    Task<string> GenerateJwtToken(string email);
}