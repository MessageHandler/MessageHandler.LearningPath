using MessageHandler.Runtime.AtomicProcessing;
using OrderBooking.Events;

namespace OrderBooking.Worker
{
    public class SendNotificationMail : IHandle<BookingStarted>
    {
        private readonly ILogger<SendNotificationMail> logger;
        private readonly ISendEmails emailSender;

        public SendNotificationMail(ISendEmails emailSender, ILogger<SendNotificationMail> logger = null)
        {
            this.logger = logger;
            this.emailSender = emailSender;
        }

        public async Task Handle(BookingStarted message, IHandlerContext context)
        {
            logger?.LogInformation("Received BookingStarted, sending notification mail...");

            await emailSender.SendAsync("sender@seller.com", "seller@seller.com", "New purchase order", "A new purchase order is available for approval");           

            logger?.LogInformation("Notification email sent");
        }
    }
}
