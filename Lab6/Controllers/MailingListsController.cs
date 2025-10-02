using Lab6.Application.Models;
using Lab6.Application.Models.MailingLists;
using Lab6.Application.Models.Shared;
using Lab6.Application.Requests.MailingLists;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Lab6.Controllers;

[ApiController]
[Route("api/mailing-lists")]
public class MailingListsController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public Task<EntityIdModel> CreateMailingList(CreateMailingListRequest request, CancellationToken cancellationToken = default)
    {
        return mediator.Send(request, cancellationToken);
    }
    
    [HttpPut("{id:guid}")]
    public Task<EntityIdModel> UpdateMailingList(Guid id, UpdateMailingListModel model, CancellationToken cancellationToken = default)
    {
        return mediator.Send(new UpdateMailingListRequest(id, model), cancellationToken);
    }
    
    [HttpGet("{id:guid}")]
    public Task<MailingListModel> GetMailingList(Guid id, CancellationToken cancellationToken = default)
    {
        return mediator.Send(new GetMailingListRequest(id), cancellationToken);
    }

    [HttpGet]
    public Task<GetItemsResponse<MailingListModel>> GetMailingLists(CancellationToken cancellationToken = default)
    {
        return mediator.Send(new GetMailingListItemsRequest(), cancellationToken);
    }

    [HttpDelete("{id:guid}")]
    public Task DeleteMailingList(Guid id, CancellationToken cancellationToken = default)
    {
        return mediator.Send(new DeleteMailingListRequest(id), cancellationToken);
    }
}

    