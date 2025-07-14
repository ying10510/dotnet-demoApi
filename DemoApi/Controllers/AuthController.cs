using Microsoft.AspNetCore.Mvc;
using DemoApi.Services;
using DemoApi.Dtos.Auth;
using Microsoft.AspNetCore.Authorization;
using Demo.Repositories;
using Demo.Exceptions;

namespace Demo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;
    private readonly IAuthService _authService;
    private readonly IMemberRepository _memberRepository;


    public AuthController(ILogger<AuthController> logger, IAuthService authService, IMemberRepository memberRepository)
    {
        _logger = logger;
        _authService = authService;
        _memberRepository = memberRepository;
    }

    /// <summary>
    /// 登入
    /// </summary>
    /// <param name="loginRequestDto"></param>
    /// <returns></returns>
    /// <exception cref="AppException"></exception>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
    {
        var member = await _memberRepository.GetByEmail(loginRequestDto.Email) ?? throw new AppException("[Login] Member Not Found", 404);

        var token = await _authService.Authenticate(loginRequestDto.Email, loginRequestDto.Password, member.Password);

        return Ok(new LoginResponseDto
        {
            Token = token,
            Email = member.Email,
            Name = member.Name
        });
    }
}
