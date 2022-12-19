using MessageHandler.EventSourcing.Contracts;

namespace OrderBooking.Events
{
    public class BookingStarted : SourcedEvent
    {
        public PurchaseOrder? PurchaseOrder { get; set; }
    }
}