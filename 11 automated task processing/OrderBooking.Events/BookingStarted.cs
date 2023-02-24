using MessageHandler.EventSourcing.Contracts;

namespace OrderBooking.Events
{
    public class BookingStarted : SourcedEvent
    {
        public string BookingId { get; set; } = string.Empty;

        public string BuyerId { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public PurchaseOrder? PurchaseOrder { get; set; }
    }
}