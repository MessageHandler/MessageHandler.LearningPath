using MessageHandler.EventSourcing.Contracts;

namespace NotificationPreferences.Events
{
    public class ConfirmationEmailSet : SourcedEvent
    {
        public string BuyerId { get; set; } = string.Empty;

        public string EmailAddress { get; set; } = string.Empty;
    }
}