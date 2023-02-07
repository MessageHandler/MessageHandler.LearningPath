using OrderBooking.Events;

namespace OrderBooking.WebAPI.Controllers
{
    public class PlacePurchaseOrder
    {
        public string BookingId { get; set; } = string.Empty;
        
        public string Name { get; set; } = string.Empty;

        public PurchaseOrder PurchaseOrder { get; set; } = new PurchaseOrder();
    }
}
