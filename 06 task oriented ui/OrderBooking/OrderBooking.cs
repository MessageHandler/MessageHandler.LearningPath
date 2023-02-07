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

        public void PlacePurchaseOrder(PurchaseOrder purchaseOrder, string name)
        {
            if (_allreadyStarted) return;

            Emit(new BookingStarted()
            {
                BookingId = Id,
                Name = name,
                PurchaseOrder = purchaseOrder
            });
        }

        public void Apply(BookingStarted msg)
        {
            _allreadyStarted = true;
        }
    }
}