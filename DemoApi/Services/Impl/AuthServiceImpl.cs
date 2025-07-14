using System.IdentityModel.Tokens.Jwt;
using DemoApi.Exceptions;

namespace DemoApi.Services.Impl;

public class AuthServiceImpl : IAuthService
{
    private readonly ILogger<AuthServiceImpl> _logger;
    private readonly IJwtService _jwtService;
    private readonly IPasswordService _passwordService;

    public AuthServiceImpl(ILogger<AuthServiceImpl> logger, IJwtService jwtService, IPasswordService passwordService)
    {
        _logger = logger;
        _jwtService = jwtService;
        _passwordService = passwordService;
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
}