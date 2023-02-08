using MessageHandler.EventSourcing.Projections;
using OrderBooking.Events;

namespace OrderBooking.Worker
{
    public class ProjectToSearch :
        IProjection<SalesOrder, BookingStarted>,
        IProjection<SalesOrder, SalesOrderConfirmed>
    {
        public void Project(SalesOrder salesOrder, BookingStarted msg)
        {
            salesOrder.Name = msg.Name;
            salesOrder.Status = "Pending";
        }

        public void Project(SalesOrder salesOrder, SalesOrderConfirmed msg)
        {
            salesOrder.Status = "Confirmed";
        }
    }
}
