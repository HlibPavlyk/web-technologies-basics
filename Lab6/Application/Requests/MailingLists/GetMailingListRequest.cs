using Lab6.Application.Models.MailingLists;
using MediatR;

namespace Lab6.Application.Requests.MailingLists;

public record GetMailingListRequest(Guid Id) : IRequest<MailingListModel>;