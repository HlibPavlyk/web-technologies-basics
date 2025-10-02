using Lab6.Application.Models.MailingLists;

namespace Lab6.Application.Models.Subscribers;

public class SubscriberMailingListModel : MailingListModel
{
    public DateTime? LastSentDate { get; set; }
}