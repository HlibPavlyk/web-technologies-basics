using Lab2.Requests.Token;
using MediatR;

namespace Lab2.Extensions;

public static class MediatorExtensions
{
    public static Task<string> CreateJwtTokenAsync(
        this IMediator mediator,
        Guid id,
        string username,
        string email,
        IEnumerable<string> roles,
        CancellationToken cancellationToken = default)
    {
        return mediator.Send(new CreateTokenRequest(id, username, email, roles.ToArray()), cancellationToken);
    }
}