using Lab2.Models.Users;
using MediatR;

namespace Lab2.Requests.Users;

public record LoginUserRequest(string EmailOrUsername, string Password) : IRequest<LoginModel>;
