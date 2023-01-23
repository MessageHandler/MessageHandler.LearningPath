using MessageHandler.EventSourcing.DomainModel;
using OrderBooking.Events;

namespace OrderBooking
{
    public class OrderBooking : EventSourced,
        IApply<BookingStarted>
    {
        private bool _allreadyStarted;

        public OrderBooking() : this(Guid.NewGuid().ToString())
        {
        }

        public OrderBooking(string id) : base(id)
        {
        }

        public void PlacePurchaseOrder(PurchaseOrder purchaseOrder)
        {
            if (_allreadyStarted) return;

            Emit(new BookingStarted()
            {
                PurchaseOrder = purchaseOrder,
                TenantId = "all"
            });
        }

        public void Apply(BookingStarted msg)
        {
            _allreadyStarted = true;
        }
    }
}