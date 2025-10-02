using Lab5.Application.Models.Subscribers;
using MediatR;

namespace Lab5.Application.Requests.Subscribers;

public record GetSubscriberRequest(Guid Id) : IRequest<SubscriberModel>;