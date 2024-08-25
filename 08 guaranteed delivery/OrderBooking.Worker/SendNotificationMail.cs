using MessageHandler.Runtime.AtomicProcessing;
using OrderBooking.Events;

namespace OrderBooking.Worker
{
    public class SendNotificationMail(
        ILogger<SendNotificationMail> _logger,
        ISendEmails _emailSender) : IHandle<BookingStarted>
    {
        private readonly ILogger<SendNotificationMail> logger = _logger;
        private readonly ISendEmails emailSender = _emailSender;

        public async Task Handle(BookingStarted message, IHandlerContext context)
        {
            logger?.LogInformation("Received BookingStarted, sending notification mail...");

            await emailSender.SendAsync("sender@seller.com", "seller@seller.com", "New purchase order", "A new purchase order is available for approval");           

            logger?.LogInformation("Notification email sent");
        }
    }
}
