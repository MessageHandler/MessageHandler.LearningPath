using MessageHandler.EventSourcing.Contracts;
using MessageHandler.EventSourcing.Projections;
using OrderBooking.Events;
using OrderBooking.Projections;
using Xunit;

namespace OrderBooking.UnitTests
{
    public class WhileProjectingBookings
    {
        [Fact]
        public void GivenBookingProcessStarted_WhenProjectingBooking_ThenBookingShouldHaveStatusStarted()
        {
            // given
            var history = new SourcedEvent[]
            {
                new BookingStarted
                {
                    PurchaseOrder = new PurchaseOrder()
                }
            };
            var booking = new Booking();

            // when
            var invoker = new ProjectionInvoker(new BookingProjection());
            invoker.Invoke(booking, history);

            // then
            Assert.Equal("Pending", booking.Status);
        }

        [Fact]
        public void GivenSalesOrderConfirmed_WhenProjectingBooking_ThenBookingShouldHaveStatusConfirmed()
        {
            // given
            var history = new SourcedEvent[]
            {
                new BookingStarted
                {
                    PurchaseOrder = new PurchaseOrder()
                },
                new SalesOrderConfirmed
                {

                }
            };
            var booking = new Booking();

            // when
            var invoker = new ProjectionInvoker(new BookingProjection());
            invoker.Invoke(booking, history);

            // then
            Assert.Equal("Confirmed", booking.Status);
        }
    }
}
