using static BCrypt.Net.BCrypt;

namespace DemoApi.Services.Impl;

public class PasswordServiceImpl : IPasswordService
{
    private readonly string _pepper;

    public PasswordServiceImpl(IConfiguration configuration)
    {
        _pepper = configuration["PasswordPepper"]
                  ?? throw new ArgumentNullException("PasswordPepper not set in environment");
    }

    /// <summary>
    /// 密碼加密
    /// </summary>
    /// <param name="password"></param>
    /// <returns></returns>
    public Task<string> PasswordHash(string password)
    {
        return Task.Run(() => HashPassword(password + _pepper));
    }

    /// <summary>
    /// 驗證密碼
    /// </summary>
    /// <param name="password"></param>
    /// <param name="hashPassword"></param>
    /// <returns></returns>
    public Task<bool> PasswordVerify(string password, string hashPassword)
    {
        return Task.Run(() => Verify(password + _pepper, hashPassword));
    }
}