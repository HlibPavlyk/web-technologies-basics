using Lab5.Application.Models.MailingLists;
using MediatR;

namespace Lab5.Application.Requests.MailingLists;

public record GetMailingListRequest(Guid Id) : IRequest<MailingListModel>;