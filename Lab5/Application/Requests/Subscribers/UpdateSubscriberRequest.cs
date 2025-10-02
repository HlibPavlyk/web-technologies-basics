using Lab5.Application.Models.Shared;
using Lab5.Application.Models.Subscribers;
using MediatR;

namespace Lab5.Application.Requests.Subscribers;

public record UpdateSubscriberRequest(Guid Id, UpdateSubscriberModel Model) : IRequest<EntityIdModel>;