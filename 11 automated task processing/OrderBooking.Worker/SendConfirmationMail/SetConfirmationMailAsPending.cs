using MessageHandler.EventSourcing.Projections;
using MessageHandler.Runtime.AtomicProcessing;
using OrderBooking.Events;

namespace OrderBooking.Worker
{
    public class SetConfirmationMailAsPending : IHandle<SalesOrderConfirmed>
    {
        private readonly IPersistConfirmationMails _client;
        private readonly IInvokeProjections<ConfirmationMail> _projection;
        private readonly ILogger<IndexConfirmationMail> _logger;

        public SetConfirmationMailAsPending(IInvokeProjections<ConfirmationMail> projection, IPersistConfirmationMails client, ILogger<IndexConfirmationMail> logger = null!)
        {
            _client = client;
            _projection = projection;
            _logger = logger;
        }

        public async Task Handle(SalesOrderConfirmed message, IHandlerContext context)
        {
            _logger?.LogInformation("Received SalesOrderConfirmed, marking the confirmation mail as pending...");

            var confirmationMail = await _client.GetConfirmationMail(message.BookingId) ?? new ConfirmationMail();

            _projection.Invoke(confirmationMail, message);

            await _client.MarkAsPending(confirmationMail);

            _logger?.LogInformation("Sales order marked as pending");
        }
    }
}
