using MediatR;

namespace Lab6.Application.Requests.Subscribers;

public record AddMailingListToSubscriberRequest(Guid Id, Guid MailingListId) : IRequest;