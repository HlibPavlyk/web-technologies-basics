using Lab5.Application.Models.Shared;
using MediatR;

namespace Lab5.Application.Requests.MailingLists;

public record DeleteMailingListRequest(Guid Id) : IRequest<EntityIdModel>;