using OrderBooking.Events;
using System.Linq;
using Xunit;

namespace OrderBooking.UnitTests
{
    public class WhilePlacingPurchaseOrders
    {
        [Fact]
        public void GivenNewBookingProcess_WhenPlacingValidPurchaseOrder_TheBookingProcessShouldHaveBeenStarted()
        {
            // given
            var booking = new OrderBooking();

            //when
            var purchaseOrder = new PurchaseOrder();
            booking.PlacePurchaseOrder(purchaseOrder, "Mr. Buyer");

            //then
            var pendingEvents = booking.Commit();
            var bookingStarted = pendingEvents.FirstOrDefault(e => typeof(BookingStarted).IsAssignableFrom(e.GetType()));

            Assert.NotNull(bookingStarted);
        }

        [Fact]
        public void GivenNewBookingProcess_WhenPlacingValidPurchaseOrderTwice_TheBookingProcessShouldHaveBeenStartedOnlyOnce()
        {
            // given
            var booking = new OrderBooking();

            //when
            var purchaseOrder = new PurchaseOrder();
            booking.PlacePurchaseOrder(purchaseOrder, "Mr. Buyer");
            booking.PlacePurchaseOrder(purchaseOrder, "Mr. Buyer");

            //then
            var pendingEvents = booking.Commit();
            var bookingStarted = pendingEvents.Where(e => typeof(BookingStarted).IsAssignableFrom(e.GetType()));

            Assert.Single(bookingStarted);
        }

        [Fact]
        public void GivenNewBookingProcess_WhenPlacingValidPurchaseOrder_ThenPurchaseOrderIsCarriedOnEvent()
        {
            // given
            var booking = new OrderBooking();

            //when
            var purchaseOrder = new PurchaseOrder();
            booking.PlacePurchaseOrder(purchaseOrder, "Mr. Buyer");

            //then
            var pendingEvents = booking.Commit();
            var bookingStarted = (BookingStarted) pendingEvents.First(e => typeof(BookingStarted).IsAssignableFrom(e.GetType()));

            Assert.NotNull(bookingStarted.BookingId);
            Assert.NotNull(bookingStarted.PurchaseOrder);
            Assert.NotNull(bookingStarted.Name);
        }
    }

}