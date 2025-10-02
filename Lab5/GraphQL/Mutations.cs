using Lab5.Application.Models.MailingLists;
using Lab5.Application.Models.Shared;
using Lab5.Application.Models.Subscribers;
using Lab5.Application.Requests.MailingLists;
using Lab5.Application.Requests.Subscribers;
using MediatR;

namespace Lab5.GraphQL;

public class Mutations
{
    #region Mailing lists
    
    public Task<EntityIdModel> CreateMailingList(CreateMailingListRequest request, [Service] IMediator mediator, CancellationToken cancellationToken = default)
    {
        return mediator.Send(request, cancellationToken);
    }
    
    public Task<EntityIdModel> UpdateMailingList(Guid id, [Service] IMediator mediator, UpdateMailingListModel model, CancellationToken cancellationToken = default)
    {
        return mediator.Send(new UpdateMailingListRequest(id, model), cancellationToken);
    }

    public Task<EntityIdModel> DeleteMailingList(Guid id, [Service] IMediator mediator, CancellationToken cancellationToken = default)
    {
        return mediator.Send(new DeleteMailingListRequest(id), cancellationToken);
    }
    
    #endregion
    
    #region Subscribers
    
    public Task<EntityIdModel> CreateSubscriber(CreateSubscriberRequest request, [Service] IMediator mediator, CancellationToken cancellationToken = default)
    {
        return mediator.Send(request, cancellationToken);
    }
    
    public Task<EntityIdModel> UpdateSubscriber(Guid id, UpdateSubscriberModel model, [Service] IMediator mediator, CancellationToken cancellationToken = default)
    {
        return mediator.Send(new UpdateSubscriberRequest(id, model), cancellationToken);
    }
    
    public Task<EntityIdModel> DeleteSubscriber(Guid id, [Service] IMediator mediator, CancellationToken cancellationToken = default)
    {
        return mediator.Send(new DeleteSubscriberRequest(id), cancellationToken);
    }
    
    public Task<EntityIdModel> AddSubscriberToMailingList(Guid id, Guid mailingListId, [Service] IMediator mediator, CancellationToken cancellationToken = default)
    {
        return mediator.Send(new AddMailingListToSubscriberRequest(id, mailingListId), cancellationToken);
    }
    
    public Task<EntityIdModel> RemoveSubscriberFromMailingList(Guid id, Guid mailingListId, [Service] IMediator mediator, CancellationToken cancellationToken = default)
    {
        return mediator.Send(new DeleteMailingListFromSubscriberRequest(id, mailingListId), cancellationToken);
    }
    
    #endregion
}