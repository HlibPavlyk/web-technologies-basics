using MediatR;

namespace Lab6.Application.Requests.MailingLists;

public record DeleteMailingListRequest(Guid Id) : IRequest;