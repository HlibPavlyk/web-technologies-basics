namespace Lab6.Application.Models.MailingLists;

public record UpdateMailingListModel
{
    public string Subject { get; set; }
    public string Content { get; set; }
}