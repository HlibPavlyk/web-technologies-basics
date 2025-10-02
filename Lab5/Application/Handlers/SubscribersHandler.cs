using Lab5.Application.Models.Shared;
using Lab5.Application.Models.Subscribers;
using Lab5.Application.Requests.Subscribers;
using Lab5.Domain.Entities;
using Lab5.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Lab5.Application.Handlers;

public class SubscribersHandler(ApplicationDbContext context) :
    IRequestHandler<CreateSubscriberRequest, EntityIdModel>,
    IRequestHandler<UpdateSubscriberRequest, EntityIdModel>,
    IRequestHandler<GetSubscriberRequest, SubscriberModel>,
    IRequestHandler<GetSubscriberItemsRequest, GetItemsResponse<SubscriberModel>>,
    IRequestHandler<DeleteSubscriberRequest, EntityIdModel>,
    IRequestHandler<GetSubscriberMailingListItemsRequest, GetItemsResponse<SubscriberMailingListModel>>,
    IRequestHandler<AddMailingListToSubscriberRequest, EntityIdModel>, 
    IRequestHandler<DeleteMailingListFromSubscriberRequest, EntityIdModel>
{
    public async Task<EntityIdModel> Handle(CreateSubscriberRequest request, CancellationToken cancellationToken)
    {
        var entity = new SubscriberEntity
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
        };

        await context.Subscribers.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return new EntityIdModel { Id = entity.Id };
    }

    public async Task<EntityIdModel> Handle(UpdateSubscriberRequest request, CancellationToken cancellationToken)
    {
        var entity = await context.Subscribers
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity is null)
            throw new Exception($"Subscriber with id {request.Id} not found");

        entity.FirstName = request.Model.FirstName;
        entity.LastName = request.Model.LastName;
        entity.Email = request.Model.Email;
        entity.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Model.Password);

        await context.SaveChangesAsync(cancellationToken);
        return new EntityIdModel { Id = entity.Id };
    }

    public async Task<SubscriberModel> Handle(GetSubscriberRequest request, CancellationToken cancellationToken)
    {
        return await context.Subscribers
            .AsNoTracking()
            .Where(x => x.Id == request.Id)
            .Select(x => new SubscriberModel
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Email = x.Email
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
    
    public async Task<GetItemsResponse<SubscriberModel>> Handle(GetSubscriberItemsRequest request, CancellationToken cancellationToken)
    {
        var items = await context.Subscribers
            .AsNoTracking()
            .Select(x => new SubscriberModel
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Email = x.Email
            })
            .ToArrayAsync(cancellationToken);

        return new GetItemsResponse<SubscriberModel>(items.Length, items);
    }

    public async Task<EntityIdModel> Handle(DeleteSubscriberRequest request, CancellationToken cancellationToken)
    {
        var entity = await context.Subscribers
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity is null)
            throw new Exception($"Subscriber with id {request.Id} not found");

        context.Subscribers.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);
        
        return new EntityIdModel { Id = entity.Id };
    }

    public async Task<GetItemsResponse<SubscriberMailingListModel>> Handle(GetSubscriberMailingListItemsRequest request, CancellationToken cancellationToken)
    {
        var items = await context.MailingListSubscribers
            .AsNoTracking()
            .Where(x => x.SubscriberId == request.Id)
            .Select(x => new SubscriberMailingListModel
            {
                Id = x.MailingListId,
                Subject = x.MailingList.Subject,
                Content = x.MailingList.Content,
                LastSentDate = x.LastSentDate
            })
            .ToArrayAsync(cancellationToken);
        
        return new GetItemsResponse<SubscriberMailingListModel>(items.Length, items);
    }

    public async Task<EntityIdModel> Handle(AddMailingListToSubscriberRequest request, CancellationToken cancellationToken)
    {
        var entity = await context.MailingListSubscribers
            .FirstOrDefaultAsync(x => x.SubscriberId == request.Id && x.MailingListId == request.MailingListId, cancellationToken);
        
        if(entity is not null)
            throw new Exception($"Mailing list with id {request.MailingListId} already exist in subscriber with id {request.Id}");
        
        entity = new MailingListSubscriberEntity
        {
            SubscriberId = request.Id,
            MailingListId = request.MailingListId,
            LastSentDate = DateTime.UtcNow
        };

        context.MailingListSubscribers.Add(entity);
        await context.SaveChangesAsync(cancellationToken);
        
        return new EntityIdModel { Id = request.MailingListId };
    }

    public async Task<EntityIdModel> Handle(DeleteMailingListFromSubscriberRequest request, CancellationToken cancellationToken)
    {
        var entity = await context.MailingListSubscribers
            .FirstOrDefaultAsync(x => x.SubscriberId == request.Id && x.MailingListId == request.MailingListId, cancellationToken);

        if(entity is null)
            throw new Exception($"Mailing list with id {request.MailingListId} not found in subscriber with id {request.Id}");

        context.MailingListSubscribers.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);
        
        return new EntityIdModel { Id = request.MailingListId };
    }
}