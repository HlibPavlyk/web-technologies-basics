using System.Security.Authentication;
using Lab2.Extensions;
using Lab2.Models.Domain;
using Lab2.Models.Users;
using Lab2.Requests.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Lab2.Handlers;

public class UsersHandler(UserManager<UserEntity> userManager, IMediator mediator) :
    IRequestHandler<RegisterUserRequest, UserModel>,
    IRequestHandler<LoginUserRequest, LoginModel>,
    IRequestHandler<UpdateUserRequest, UserModel>
{
    public async Task<UserModel> Handle(RegisterUserRequest request, CancellationToken cancellationToken)
    {
        var user = new UserEntity(request.Username.Trim(), request.Email.ToLower());

        var result = await userManager.CreateAsync(user, request.Password);

        if (result.Succeeded)
        {
            result = await userManager.AddToRoleAsync(user, request.IsAdmin ? "Administrator" : "User");
            if (result.Succeeded)
            {
                return await GetUserModelAsync(user, cancellationToken);
            }
        }

        throw new AuthenticationException("User creation failed");
    }

    public async Task<LoginModel> Handle(LoginUserRequest request, CancellationToken cancellationToken)
    {
        var identityUser = await userManager.FindByEmailAsync(request.EmailOrUsername)
                           ?? await userManager.FindByNameAsync(request.EmailOrUsername);

        if (identityUser != null)
        {
            var result = await userManager.CheckPasswordAsync(identityUser, request.Password);

            if (result)
            {
                var roles = await userManager.GetRolesAsync(identityUser);

                if (identityUser.Email != null && identityUser.UserName != null)
                {
                    var jwtToken = await mediator.CreateJwtTokenAsync(identityUser.Id, identityUser.UserName,
                        identityUser.Email, roles, cancellationToken);

                    return new LoginModel
                    {
                        Id = identityUser.Id,
                        Username = identityUser.UserName,
                        Email = identityUser.Email,
                        Roles = roles.ToArray(),
                        Token = jwtToken
                    };
                }
            }
        }

        throw new AuthenticationException("Invalid email or password");
    }
    
    public async Task<UserModel> Handle(UpdateUserRequest request, CancellationToken cancellationToken)
    {
        var user = await userManager.GetUserAsync(request.User)
                   ?? throw new AuthenticationException("Current User not found");

        // apply optional updates
        if (!string.IsNullOrWhiteSpace(request.Model.Username) && request.Model.Username != user.UserName)
            user.UserName = request.Model.Username.Trim();

        if (!string.IsNullOrWhiteSpace(request.Model.Email) && request.Model.Email != user.Email)
            user.Email = request.Model.Email.ToLower();

        var updateResult = await userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
            throw new AuthenticationException("Failed to update user profile");

        // update password if provided
        if (!string.IsNullOrWhiteSpace(request.Model.Password))
        {
            // remove existing password (if any), then set new password
            var removed = await userManager.RemovePasswordAsync(user);
            if (!removed.Succeeded && removed.Errors.Any(e => e.Code != "UserHasNoPassword"))
                throw new AuthenticationException("Failed to remove existing password");

            var added = await userManager.AddPasswordAsync(user, request.Model.Password);
            if (!added.Succeeded)
                throw new AuthenticationException("Failed to set new password");
        }

        // update roles if requested
        var roles = await userManager.GetRolesAsync(user);
        var isCurrentlyAdmin = roles.Contains("Administrator");
        if (request.Model.IsAdmin && !isCurrentlyAdmin)
        {
            await userManager.RemoveFromRoleAsync(user, "User");
            var roleResult = await userManager.AddToRoleAsync(user, "Administrator");
            if (!roleResult.Succeeded) throw new AuthenticationException("Failed to set Administrator role");
        }
        else if (!request.Model.IsAdmin && isCurrentlyAdmin)
        {
            await userManager.RemoveFromRoleAsync(user, "Administrator");
            var roleResult = await userManager.AddToRoleAsync(user, "User");
            if (!roleResult.Succeeded) throw new AuthenticationException("Failed to set User role");
        }

        return await GetUserModelAsync(user, cancellationToken);
    }

    private async Task<UserModel> GetUserModelAsync(UserEntity identityUser, CancellationToken cancellationToken)
    {
        var roles = await userManager.GetRolesAsync(identityUser);

        if (identityUser.Email != null && identityUser.UserName != null)
        {
            return new UserModel
            {
                Id = identityUser.Id,
                Username = identityUser.UserName,
                Email = identityUser.Email,
                Roles = roles.ToArray(),
            };
        }

        throw new AuthenticationException("Invalid email or password");
    }
}