using MessageHandler.Runtime.AtomicProcessing;
using OrderBooking.Events;
using PostmarkDotNet;

namespace OrderBooking.Worker
{
    public class SendNotificationMail : IHandle<BookingStarted>
    {
        private readonly ILogger<SendNotificationMail> logger;

        public SendNotificationMail(ILogger<SendNotificationMail> logger = null)
        {
            this.logger = logger;
        }

        public async Task Handle(BookingStarted message, IHandlerContext context)
        {
            logger.LogInformation("Received BookingStarted, sending notification mail...");

            var email = new PostmarkMessage()
            {
                From = "sender@seller.com",
                To = "seller@seller.com",
                Subject = "A new purchase order is available for approval",
                HtmlBody = "A new purchase order is available for approval",
                TextBody = "A new purchase order is available for approval"
            };

            var client = new PostmarkClient("POSTMARK_API_TEST");
            var response = await client.SendMessageAsync(email);

            logger.LogInformation("Notification mail status: " + response.Status);
        }
    }
}
