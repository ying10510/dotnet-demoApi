namespace DemoApi.Services;

public interface ITokenBlacklistService
{
    Task AddTokenToBlacklist(string token, TimeSpan? expiry);
    Task<bool> IsTokenBlacklisted(string token);
}