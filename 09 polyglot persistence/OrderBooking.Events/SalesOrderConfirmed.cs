using MessageHandler.EventSourcing.Contracts;

namespace OrderBooking.Events
{
    public class SalesOrderConfirmed : SourcedEvent
    {
        public string Id { get; set; } = string.Empty;
    }
}