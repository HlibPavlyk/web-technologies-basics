using MediatR;

namespace Lab6.Application.Requests.Subscribers;

public record DeleteSubscriberRequest(Guid Id) : IRequest;