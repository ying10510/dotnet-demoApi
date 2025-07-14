namespace DemoApi.Services;

public interface IPasswordService
{
    Task<string> PasswordHash(string password);
    Task<bool> PasswordVerify(string password, string hashPassword);
}