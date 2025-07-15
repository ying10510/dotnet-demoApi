using StackExchange.Redis;

namespace DemoApi.Services.Impl;

public class TokenBlacklistServiceImpl : ITokenBlacklistService
{
    private readonly string _blacklistPrefix;
    private static readonly TimeSpan _defaultExpiry = TimeSpan.FromHours(1);
    private readonly IDatabase _db;

    public TokenBlacklistServiceImpl(IConnectionMultiplexer redis, IConfiguration configuration)
    {
        _db = redis.GetDatabase();
        _blacklistPrefix = configuration["RedisKey"]
            ?? throw new ArgumentNullException("RedisKey does not set in environment");
    }

    /// <summary>
    /// 將 token 存進 redis 並設定過期時間
    /// </summary>
    /// <param name="token"></param>
    /// <param name="expiry"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public async Task AddTokenToBlacklist(string token, TimeSpan? expiry)
    {
        if (string.IsNullOrWhiteSpace(token)) throw new ArgumentNullException(nameof(token));
        await _db.StringSetAsync($"{_blacklistPrefix}:{token}", "revoked", expiry ?? _defaultExpiry);
    }

    /// <summary>
    /// 檢查 token 是否已被加入 Redis 黑名單
    /// </summary>
    /// <param name="token"></param>
    /// <returns>
    ///     true-被黑名單，為無效token
    ///     false-未被加入，為有效token或已過期token
    /// </returns>
    public async Task<bool> IsTokenBlacklisted(string token)
    {
        if (string.IsNullOrWhiteSpace(token)) throw new ArgumentNullException(nameof(token));
        return await _db.KeyExistsAsync($"{_blacklistPrefix}:{token}");
    }
}