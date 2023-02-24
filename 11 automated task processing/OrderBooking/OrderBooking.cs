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

            Emit(new BookingStarted()
            {
                BookingId = Id,
                BuyerId = buyerId,
                Name = name,
                PurchaseOrder = purchaseOrder
            });
        }

        public void ConfirmSalesOrder()
        {
            if (_confirmed) return;

            Emit(new SalesOrderConfirmed()
            {
                BookingId = Id
            });
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