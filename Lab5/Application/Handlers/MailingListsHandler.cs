using Lab5.Application.Models.MailingLists;
using Lab5.Application.Models.Shared;
using Lab5.Application.Requests.MailingLists;
using Lab5.Domain.Entities;
using Lab5.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Lab5.Application.Handlers;

public class MailingListsHandler(ApplicationDbContext context) :
    IRequestHandler<CreateMailingListRequest, EntityIdModel>,
    IRequestHandler<UpdateMailingListRequest, EntityIdModel>,
    IRequestHandler<GetMailingListRequest, MailingListModel>,
    IRequestHandler<GetMailingListItemsRequest, GetItemsResponse<MailingListModel>>,
    IRequestHandler<DeleteMailingListRequest, EntityIdModel>
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

    public async Task<EntityIdModel> Handle(DeleteMailingListRequest request, CancellationToken cancellationToken)
    {
        var entity = await context.MailingLists
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity is null)
            throw new Exception($"Mailing list with id {request.Id} not found");

        context.MailingLists.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);
        
        return new EntityIdModel { Id = entity.Id };
    }

}