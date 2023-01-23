using OrderBooking.Events;

namespace OrderBooking.WebAPI.Controllers
{
    public class PlacePurchaseOrder
    {
        public PurchaseOrder PurchaseOrder { get; set; } = new PurchaseOrder();
    }
}
