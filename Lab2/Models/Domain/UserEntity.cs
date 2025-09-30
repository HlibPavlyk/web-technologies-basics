using Microsoft.AspNetCore.Identity;

namespace Lab2.Models.Domain;

public sealed class UserEntity : IdentityUser<Guid>
{
    public UserEntity(string userName, string email) : base(userName)
    {
        Email = email;
    }
}