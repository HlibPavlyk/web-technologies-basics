namespace Lab5.Domain.Entities;

public class SubscriberEntity
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    
    // navigation properties
    public ICollection<MailingListSubscriberEntity> MailingListSubscribers { get; set; } = [];
}