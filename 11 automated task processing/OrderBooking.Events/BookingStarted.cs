using MessageHandler.EventSourcing.Contracts;

namespace OrderBooking.Events;

public class BookingStarted(
    string bookingId,
    string buyerId,
    string name,
    PurchaseOrder purchaseOrder) : SourcedEvent
{
    public string BookingId => bookingId;
    public string BuyerId => buyerId;
    public string Name => name;
    public PurchaseOrder PurchaseOrder => purchaseOrder;
}