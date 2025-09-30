using MediatR;

namespace Lab2.Requests.Token;

public record CreateTokenRequest(Guid Id, string Username, string Email, string[] Roles) : IRequest<string>;
