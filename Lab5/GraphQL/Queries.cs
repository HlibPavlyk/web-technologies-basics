using Lab5.Application.Models.MailingLists;
using Lab5.Application.Models.Shared;
using Lab5.Application.Models.Subscribers;
using Lab5.Application.Requests.MailingLists;
using Lab5.Application.Requests.Subscribers;
using MediatR;

namespace Lab5.GraphQL;

public class Queries
{
    #region Mailing lists
    
    public Task<MailingListModel> GetMailingList(Guid id, [Service] IMediator mediator, CancellationToken cancellationToken = default)
    {
        return mediator.Send(new GetMailingListRequest(id), cancellationToken);
    }
    
    public Task<GetItemsResponse<MailingListModel>> GetMailingLists([Service] IMediator mediator, CancellationToken cancellationToken = default)
    {
        return mediator.Send(new GetMailingListItemsRequest(), cancellationToken);
    }
    
    #endregion
    
    #region Subscribers
    
    public Task<SubscriberModel> GetSubscriber(Guid id, [Service] IMediator mediator, CancellationToken cancellationToken = default)
    {
        return mediator.Send(new GetSubscriberRequest(id), cancellationToken);
    }
    
    public Task<GetItemsResponse<SubscriberModel>> GetSubscribers([Service] IMediator mediator, CancellationToken cancellationToken = default)
    {
        return mediator.Send(new GetSubscriberItemsRequest(), cancellationToken);
    }
    
    public Task<GetItemsResponse<SubscriberMailingListModel>> GetSubscriberMailingLists([Service] IMediator mediator, Guid id, CancellationToken cancellationToken = default)
    {
        return mediator.Send(new GetSubscriberMailingListItemsRequest(id), cancellationToken);
    }
    
    #endregion
}