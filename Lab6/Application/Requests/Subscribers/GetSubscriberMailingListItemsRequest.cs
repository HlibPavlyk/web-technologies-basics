using Lab6.Application.Models.Shared;
using Lab6.Application.Models.Subscribers;
using MediatR;

namespace Lab6.Application.Requests.Subscribers;

public record GetSubscriberMailingListItemsRequest(Guid Id) : IRequest<GetItemsResponse<SubscriberMailingListModel>>;