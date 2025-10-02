using Lab6.Application.Models.Subscribers;
using MediatR;

namespace Lab6.Application.Requests.Subscribers;

public record GetSubscriberRequest(Guid Id) : IRequest<SubscriberModel>;