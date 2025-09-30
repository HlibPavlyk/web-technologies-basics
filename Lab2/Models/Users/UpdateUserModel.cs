using System.ComponentModel.DataAnnotations;

namespace Lab2.Models.Users;

public record UpdateUserModel
{
    public string Username { get; set; }
    [EmailAddress]
    public string Email { get; set; }
    public string Password { get; set; }
    public bool IsAdmin { get; set; }
}