using Lab5.Application.Models.Shared;
using MediatR;

namespace Lab5.Application.Requests.Subscribers;

public record DeleteSubscriberRequest(Guid Id) : IRequest<EntityIdModel>;