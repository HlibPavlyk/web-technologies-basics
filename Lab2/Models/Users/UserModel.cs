namespace Lab2.Models.Users;

public class UserModel
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string[] Roles { get; set; }
}