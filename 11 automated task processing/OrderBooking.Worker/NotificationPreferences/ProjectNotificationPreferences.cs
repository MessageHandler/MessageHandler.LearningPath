using MessageHandler.EventSourcing.Projections;
using NotificationPreferences.Events;

namespace OrderBooking.Worker
{
    public class ProjectNotificationPreferences :
        IProjection<NotificationPreferences, ConfirmationEmailSet>
    {
        public void Project(NotificationPreferences model, ConfirmationEmailSet evt)
        {
            model.BuyerId = evt.BuyerId;
            model.EmailAddress = evt.EmailAddress;
        }
    }
}
