namespace DemoApi.Services;

public interface ITokenBlacklistService : IAppService
{
    Task AddTokenToBlacklist(string token, TimeSpan? expiry);
    Task<bool> IsTokenBlacklisted(string token);
}