using MessageHandler.EventSourcing.Contracts;

namespace OrderBooking.Events
{
    public class SalesOrderConfirmed : SourcedEvent
    {
        public string BookingId { get; set; } = string.Empty;
    }
}