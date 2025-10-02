using Lab5.Application.Models.MailingLists;
using Lab5.Application.Models.Shared;
using MediatR;

namespace Lab5.Application.Requests.MailingLists;

public record CreateMailingListRequest : UpdateMailingListModel, IRequest<EntityIdModel>;