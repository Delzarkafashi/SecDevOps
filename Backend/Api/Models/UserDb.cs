namespace Api.Models;

public class UserDb
{
    public long Id { get; set; }
    public string Email { get; set; } = "";
    public string Username { get; set; } = "";
    public string Password_Hash { get; set; } = "";
    public string Role { get; set; } = "";
    public bool Is_Active { get; set; }
    public int Failed_Login_Count { get; set; }
    public DateTime? Locked_Until { get; set; }
}