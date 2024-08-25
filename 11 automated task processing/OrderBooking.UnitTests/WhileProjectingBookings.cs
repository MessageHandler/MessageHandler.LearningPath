using MessageHandler.EventSourcing.Contracts;
using MessageHandler.EventSourcing.Testing;
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
                new BookingStarted("", "", "", new PurchaseOrder(1))
            };
            var booking = new Booking();

            // when
            var invoker = new TestProjectionInvoker<Booking>(new BookingProjection());
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
                new  BookingStarted("", "", "", new PurchaseOrder(1)),
                new SalesOrderConfirmed("")
            };
            var booking = new Booking();

            // when
            var invoker = new TestProjectionInvoker<Booking>(new BookingProjection());
            invoker.Invoke(booking, history);

            // then
            Assert.Equal("Confirmed", booking.Status);
        }
    }
}
