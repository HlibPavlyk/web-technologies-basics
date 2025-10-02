namespace Lab6.Domain.Entities;

public class MailingListEntity
{
    public Guid Id { get; set; }
    public string Subject { get; set; }
    public string Content { get; set; }

    // navigation properties
    public ICollection<MailingListSubscriberEntity> MailingListSubscribers { get; set; } = [];
}