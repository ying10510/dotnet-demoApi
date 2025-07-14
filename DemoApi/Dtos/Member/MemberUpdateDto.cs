using System.ComponentModel.DataAnnotations;

namespace DemoApi.Dtos.Member;

public class MemberUpdateDto
{
    [Required]
    public required string Name { get; set; }

    [Required]
    [EmailAddress]
    public required string Email { get; set; }

    [Phone]
    public string Phone { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;
}