using Lab6.Application.Models.MailingLists;
using Lab6.Application.Models.Shared;
using MediatR;

namespace Lab6.Application.Requests.MailingLists;

public record GetMailingListItemsRequest : IRequest<GetItemsResponse<MailingListModel>>;