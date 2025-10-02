namespace Lab5.Domain.Entities;

public class MailingListSubscriberEntity
{
    public Guid MailingListId { get; set; }
    public Guid SubscriberId { get; set; }
    public DateTime LastSentDate { get; set; }
    
    // navigation properties
    public MailingListEntity MailingList { get; set; }
    public SubscriberEntity Subscriber { get; set; }


}