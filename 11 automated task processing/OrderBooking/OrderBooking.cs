using MessageHandler.EventSourcing.DomainModel;
using OrderBooking.Events;

namespace OrderBooking
{
    public class OrderBooking : EventSourced,
        IApply<BookingStarted>,
        IApply<SalesOrderConfirmed>
    {
        private bool _allreadyStarted;
        private bool _confirmed;

        public OrderBooking() : this(Guid.NewGuid().ToString())
        {
        }

        public OrderBooking(string id) : base(id)
        {
        }

        public void PlacePurchaseOrder(PurchaseOrder purchaseOrder, string buyerId, string name)
        {
            if (_allreadyStarted) return;

            Emit(new BookingStarted(Id, buyerId, name, purchaseOrder));
        }

        public void ConfirmSalesOrder()
        {
            if (_confirmed) return;

            Emit(new SalesOrderConfirmed(Id));
        }

        public void Apply(BookingStarted msg)
        {
            _allreadyStarted = true;
        }

        public void Apply(SalesOrderConfirmed msg)
        {
            _confirmed = true;
        }
    }
}