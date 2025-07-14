using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace DemoApi.Services.Impl;

public class JwtServiceImpl : IJwtService
{

    private readonly IConfiguration _configuration;

    public JwtServiceImpl(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// 建立 JWT Token
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    public async Task<string> GenerateJwtToken(string email)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, email),   // 用 Email 作為識別
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())   // JWT ID 唯一標識
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));    // 密鑰
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);     //  簽名演算法

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],   // 發行者
            audience: _configuration["Jwt:Audience"],   // 受眾
            claims: claims, // 聲明
            expires: DateTime.Now.AddHours(1),   // 過期時間
            signingCredentials: creds); // 簽名憑證


        return await Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
    }
}