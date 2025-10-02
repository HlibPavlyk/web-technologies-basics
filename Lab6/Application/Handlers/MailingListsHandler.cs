using Lab6.Application.Models;
using Lab6.Application.Models.MailingLists;
using Lab6.Application.Models.Shared;
using Lab6.Application.Requests.MailingLists;
using Lab6.Infrastructure;
using MediatR;
using Lab6.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lab6.Application.Handlers;

public class MailingListsHandler(ApplicationDbContext context) :
    IRequestHandler<CreateMailingListRequest, EntityIdModel>,
    IRequestHandler<UpdateMailingListRequest, EntityIdModel>,
    IRequestHandler<GetMailingListRequest, MailingListModel>,
    IRequestHandler<GetMailingListItemsRequest, GetItemsResponse<MailingListModel>>,
    IRequestHandler<DeleteMailingListRequest>
{
    public async Task<EntityIdModel> Handle(CreateMailingListRequest request, CancellationToken cancellationToken)
    {
        var entity = new MailingListEntity
        {
            Id = Guid.NewGuid(),
            Subject = request.Subject,
            Content = request.Content
        };

        await context.MailingLists.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return new EntityIdModel { Id = entity.Id };
    }

    public async Task<EntityIdModel> Handle(UpdateMailingListRequest request, CancellationToken cancellationToken)
    {
        var entity = await context.MailingLists
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity is null)
            throw new Exception($"Mailing list with id {request.Id} not found");

        entity.Subject = request.Model.Subject;
        entity.Content = request.Model.Content;

        await context.SaveChangesAsync(cancellationToken);
        return new EntityIdModel { Id = entity.Id };
    }

    public async Task<MailingListModel> Handle(GetMailingListRequest request, CancellationToken cancellationToken)
    {
        return await context.MailingLists
            .AsNoTracking()
            .Where(x => x.Id == request.Id)
            .Select(x => new MailingListModel
            {
                Id = x.Id,
                Subject = x.Subject,
                Content = x.Content
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
    
    public async Task<GetItemsResponse<MailingListModel>> Handle(GetMailingListItemsRequest request, CancellationToken cancellationToken)
    {
        var items = await context.MailingLists
            .AsNoTracking()
            .Select(x => new MailingListModel
            {
                Id = x.Id,
                Subject = x.Subject,
                Content = x.Content
            })
            .ToArrayAsync(cancellationToken);

        return new GetItemsResponse<MailingListModel>(items.Length, items);
    }

    public async Task Handle(DeleteMailingListRequest request, CancellationToken cancellationToken)
    {
        var entity = await context.MailingLists
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity is null)
            throw new Exception($"Mailing list with id {request.Id} not found");

        context.MailingLists.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);
    }

}