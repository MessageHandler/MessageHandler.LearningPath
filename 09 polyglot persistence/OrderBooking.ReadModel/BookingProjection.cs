using MessageHandler.EventSourcing.Projections;
using OrderBooking.Events;

namespace OrderBooking.Projections
{
    public class BookingProjection :
        IProjection<Booking, BookingStarted>,
        IProjection<Booking, SalesOrderConfirmed>
    {
        public void Project(Booking booking, BookingStarted msg)
        {
            booking.Status = "Pending";
        }

        public void Project(Booking booking, SalesOrderConfirmed msg)
        {
            booking.Status = "Confirmed";
        }
    }
}
