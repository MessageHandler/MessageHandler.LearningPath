using MessageHandler.EventSourcing.Contracts;

namespace OrderBooking.Events;

public class SalesOrderConfirmed(string bookingId) : SourcedEvent
{
    public string BookingId => bookingId;
}