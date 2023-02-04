using MessageHandler.EventSourcing.Contracts;

namespace OrderBooking.Events
{
    public class BookingStarted : SourcedEvent
    {
        public string Id { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public PurchaseOrder? PurchaseOrder { get; set; }
    }
}