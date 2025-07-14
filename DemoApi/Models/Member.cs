using System.ComponentModel.DataAnnotations;

namespace DemoApi.Models;

public class Member
{
    [Key]
    [StringLength(32)]
    public string Id { get; private set; }

    [StringLength(255)]
    public string Name { get; private set; } = string.Empty;

    [StringLength(255)]
    [EmailAddress]
    public string Email { get; private set; }

    [StringLength(20)]
    [Phone]
    public string Phone { get; private set; } = string.Empty;

    [StringLength(255)]
    public string Address { get; private set; } = string.Empty;

    [StringLength(255)]
    public string Password { get; private set; }

    public DateTime CreateTime { get; private set; }

    public DateTime UpdateTime { get; private set; }


    public Member(string email, string password)
    {
        Id = Guid.NewGuid().ToString("N");
        Email = email;
        Password = password;
        CreateTime = DateTime.Now;
        UpdateTime = DateTime.Now;
    }

    public void SetName(string name) { Name = name; }
    public void SetEmail(string email) { Email = email; }
    public void SetPhone(string phone) { Phone = phone; }
    public void SetAddress(string address) { Address = address; }
    public void SetPassword(string password) { Password = password; }
    public void SetUpdateTime(DateTime updateTime) { UpdateTime = updateTime; }

}