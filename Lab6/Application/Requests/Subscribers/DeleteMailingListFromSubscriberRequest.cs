using MediatR;

namespace Lab6.Application.Requests.Subscribers;

public record DeleteMailingListFromSubscriberRequest(Guid Id, Guid MailingListId) : IRequest;