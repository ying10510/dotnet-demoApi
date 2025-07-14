using System.ComponentModel.DataAnnotations;

namespace DemoApi.Dtos.Auth;

public class LoginRequestDto
{
    [Required]
    public required string Email { get; set; }

    [Required]
    public required string Password { get; set; }
}