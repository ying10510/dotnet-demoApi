namespace DemoApi.Dtos.Auth;

public class LoginResponseDto
{
    public required string Token { get; set; }
    public required string Email { get; set; }
    public required string Name { get; set; }
}