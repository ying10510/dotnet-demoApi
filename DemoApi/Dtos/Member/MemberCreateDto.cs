using System.ComponentModel.DataAnnotations;

namespace Demo.Dtos.Member;

public class MemberCreateDto
{
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public required string Email { get; set; }

    [Phone]
    public string Phone { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    [Required]
    public required string Password { get; set; }
}