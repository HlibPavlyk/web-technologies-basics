using System.Security.Claims;
using Lab2.Models.Users;
using MediatR;

namespace Lab2.Requests.Users;

public record UpdateUserRequest(ClaimsPrincipal User, UpdateUserModel Model) : IRequest<UserModel>;
