using Lab6.Application.Models;
using Lab6.Application.Models.Shared;
using Lab6.Application.Models.Subscribers;
using MediatR;

namespace Lab6.Application.Requests.Subscribers;

public record UpdateSubscriberRequest(Guid Id, UpdateSubscriberModel Model) : IRequest<EntityIdModel>;