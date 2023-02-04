using OrderBooking.Events;

namespace OrderBooking.WebAPI.Controllers
{
    public class PlacePurchaseOrder
    {
        public string Name { get; set; } = string.Empty;

        public PurchaseOrder PurchaseOrder { get; set; } = new PurchaseOrder();
    }
}
