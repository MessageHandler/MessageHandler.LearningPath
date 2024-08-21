using MessageHandler.EventSourcing.DomainModel;
using NotificationPreferences.Events;

namespace NotificationPreferences
{
    public class NotificationPreferences : EventSourced,
        IApply<ConfirmationEmailSet>
    {
        private string _email = string.Empty;

        public NotificationPreferences() : this(Guid.NewGuid().ToString())
        {
        }

        public NotificationPreferences(string id) : base(id)
        {
        }

        public void SetConfirmationEmail(string email)
        {
            if (_email == email) return;

            Emit(new ConfirmationEmailSet()
            {
                BuyerId = Id,
                EmailAddress = email
            });
        }

        public void Apply(ConfirmationEmailSet evt)
        {
            _email = evt.EmailAddress;
        }
    }
}