namespace DemoApi.Dtos.Member;

public class MemberReadDto
{
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public DateTime CreateTime { get; set; }

    public DateTime UpdateTime { get; set; }
}