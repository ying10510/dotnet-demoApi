using System.IdentityModel.Tokens.Jwt;
using DemoApi.Exceptions;

namespace DemoApi.Services.Impl;

public class AuthServiceImpl : IAuthService
{
    private readonly ILogger<AuthServiceImpl> _logger;
    private readonly IJwtService _jwtService;
    private readonly IPasswordService _passwordService;
    private readonly ITokenBlacklistService _tokenBlacklistService;

    public AuthServiceImpl(ILogger<AuthServiceImpl> logger, IJwtService jwtService, IPasswordService passwordService, ITokenBlacklistService tokenBlacklistService)
    {
        _logger = logger;
        _jwtService = jwtService;
        _passwordService = passwordService;
        _tokenBlacklistService = tokenBlacklistService;
    }

    /// <summary>
    /// 生成 JWT Token
    /// </summary>
    /// <param name="email"></param>
    /// <param name="password"></param>
    /// <param name="passwordHash"></param>
    /// <returns></returns>
    /// <exception cref="AppException"></exception>
    public async Task<string> Authenticate(string email, string password, string passwordHash)
    {
        // 驗證密碼
        var IsVerify = await _passwordService.PasswordVerify(password, passwordHash);
        if (!IsVerify)
        {
            throw new AppException("[Authenticate] Invalid Password", 401);
        }

        // 生成 JWT Token
        var token = await _jwtService.GenerateJwtToken(email) ?? throw new AppException("Generate JWT Token Fail");
        return token;
    }

    /// <summary>
    /// 登出
    /// </summary>
    /// <param name="authorizationHeader"></param>
    /// <returns></returns>
    /// <exception cref="AppException"></exception>
    public async Task Logout(string authorizationHeader)
    {
        _logger.LogInformation($"[Logout] authorizationHeader: {authorizationHeader}");

        if (string.IsNullOrWhiteSpace(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
        {
            throw new AppException("[Logout] Authorization header missing or invalid", 400);
        }

        var token = authorizationHeader["Bearer ".Length..].Trim();
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new AppException("[Logout] Token is missing", 400);
        }

        _logger.LogInformation($"[Logout] token: {token}");

        // 取得 token 過期時間
        JwtSecurityToken jwtToken;
        try
        {
            var jwtHandler = new JwtSecurityTokenHandler();
            jwtToken = jwtHandler.ReadJwtToken(token);
        }
        catch (Exception ex)
        {
            throw new AppException($"[Logout] Invalid token format: {ex.Message}", 400);
        }


        var expiry = jwtToken.ValidTo.ToLocalTime() - DateTime.Now;
        if (expiry <= TimeSpan.Zero)
        {
            throw new AppException("Token already expired", 400);
        }

        // 將 token 加入黑名單
        await _tokenBlacklistService.AddTokenToBlacklist(token, expiry);
    }
}