using Lab5.Application.Models.MailingLists;

namespace Lab5.Application.Models.Subscribers;

public class SubscriberMailingListModel : MailingListModel
{
    public DateTime? LastSentDate { get; set; }
}