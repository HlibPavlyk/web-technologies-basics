using Lab6.Application.Models.Shared;
using Lab6.Application.Models.Subscribers;
using Lab6.Application.Requests.Subscribers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Lab6.Controllers;

[ApiController]
[Route("api/subscribers")]
public class SubscribersController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public Task<EntityIdModel> CreateSubscriber(CreateSubscriberRequest request, CancellationToken cancellationToken = default)
    {
        return mediator.Send(request, cancellationToken);
    }
    
    [HttpPut("{id:guid}")]
    public Task<EntityIdModel> UpdateSubscriber(Guid id, UpdateSubscriberModel model, CancellationToken cancellationToken = default)
    {
        return mediator.Send(new UpdateSubscriberRequest(id, model), cancellationToken);
    }
    
    [HttpGet("{id:guid}")]
    public Task<SubscriberModel> GetSubscriber(Guid id, CancellationToken cancellationToken = default)
    {
        return mediator.Send(new GetSubscriberRequest(id), cancellationToken);
    }
    
    [HttpGet]
    public Task<GetItemsResponse<SubscriberModel>> GetSubscribers(CancellationToken cancellationToken = default)
    {
        return mediator.Send(new GetSubscriberItemsRequest(), cancellationToken);
    }
    
    [HttpDelete("{id:guid}")]
    public Task DeleteSubscriber(Guid id, CancellationToken cancellationToken = default)
    {
        return mediator.Send(new DeleteSubscriberRequest(id), cancellationToken);
    }
    
    [HttpPost("{id:guid}/mailing-lists/{mailingListId:guid}")]
    public Task AddSubscriberToMailingList(Guid id, Guid mailingListId, CancellationToken cancellationToken = default)
    {
        return mediator.Send(new AddMailingListToSubscriberRequest(id, mailingListId), cancellationToken);
    }
    
    [HttpDelete("{id:guid}/mailing-lists/{mailingListId:guid}")]
    public Task RemoveSubscriberFromMailingList(Guid id, Guid mailingListId, CancellationToken cancellationToken = default)
    {
        return mediator.Send(new DeleteMailingListFromSubscriberRequest(id, mailingListId), cancellationToken);
    }
    
    [HttpGet("{id:guid}/mailing-lists")]
    public Task<GetItemsResponse<SubscriberMailingListModel>> GetSubscriberMailingLists(Guid id, CancellationToken cancellationToken = default)
    {
        return mediator.Send(new GetSubscriberMailingListItemsRequest(id), cancellationToken);
    }
}