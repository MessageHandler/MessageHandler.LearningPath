using OrderBooking.Events;

namespace OrderBooking.WebAPI.Controllers;

public record PlacePurchaseOrder(
    string BookingId,
    string BuyerId,
    string Name,
    PurchaseOrder PurchaseOrder);
