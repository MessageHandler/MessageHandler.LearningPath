using MessageHandler.EventSourcing.Projections;
using MessageHandler.Runtime.AtomicProcessing;
using NotificationPreferences.Events;

namespace OrderBooking.Worker
{
    public class IndexNotificationPreferences : IHandle<ConfirmationEmailSet>
    {
        private readonly IPersistNotificationPreferences _client;
        private readonly IInvokeProjections<NotificationPreferences> _projection;
        private readonly ILogger<IndexNotificationPreferences> _logger;

        public IndexNotificationPreferences(IInvokeProjections<NotificationPreferences> projection, IPersistNotificationPreferences client, ILogger<IndexNotificationPreferences> logger = null!)
        {
            _client = client;
            _projection = projection;
            _logger = logger;
        }

        public async Task Handle(ConfirmationEmailSet message, IHandlerContext context)
        {
            _logger?.LogInformation("Received ConfirmationEmailSet, indexing the notification preferences...");

            var insert = false;
            var preferences = await _client.GetNotificationPreferences(message.BuyerId);
            if (preferences == null)
            {
                preferences = new NotificationPreferences();
                insert = true;
            }

            _projection.Invoke(preferences, message);

            if(insert)
            {
                await _client.Insert(preferences);
            }
            else
            {
                await _client.Update(preferences);
            }            

            _logger?.LogInformation("Notification preferences indexed");
        }
    }
}
