using Lab2.Models.Users;
using Lab2.Requests.Users;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lab2.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController(IMediator mediator) : ControllerBase
{
    [HttpPost("register")]
    public Task<UserModel> Register(RegisterUserRequest request, CancellationToken cancellationToken = default)
    {
        return mediator.Send(request, cancellationToken);
    }

    [HttpPost("login")]
    public Task<LoginModel> Login(LoginUserRequest request, CancellationToken cancellationToken = default)
    {
        return mediator.Send(request, cancellationToken);
    }
    
    [HttpPatch]
    [Authorize]
    public Task<UserModel> Login(UpdateUserModel model, CancellationToken cancellationToken = default)
    {
        return mediator.Send(new UpdateUserRequest(User, model), cancellationToken);
    }
}