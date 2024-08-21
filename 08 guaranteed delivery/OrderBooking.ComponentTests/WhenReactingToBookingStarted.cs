using MessageHandler.EventSourcing.Contracts;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Moq;
using OrderBooking.Events;
using OrderBooking.WebAPI.SignalR;
using OrderBooking.Worker;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace OrderBooking.ComponentTests
{
    public class WhenReactingToBookingStarted
    {
        [Fact]
        public async Task GivenBookingStarted_WhenNotifyingTheSeller_ShouldForwardMessageToSignal()
        {
            // given
            var bookingStarted = new BookingStarted();

            // mock signalr
            var mockGroups = new Mock<IClientProxy>();
            mockGroups.Setup(_ => _.SendCoreAsync("Notify", It.Is<object?[]>(o => o.Contains(bookingStarted)), It.IsAny<CancellationToken>())).Verifiable();

            var mockClients = new Mock<IHubClients>();
            mockClients.Setup(_ => _.Group("all")).Returns(mockGroups.Object).Verifiable();

            Mock<IHubContext<EventsHub>> mockContext = new Mock<IHubContext<EventsHub>>();
            mockContext.Setup(_ => _.Clients).Returns(mockClients.Object).Verifiable();            

            //when
            var reaction = new NotifySeller(mockContext.Object);
            await reaction.Push(new[] { bookingStarted });

            // Then
            mockClients.Verify();
            mockGroups.Verify();
            mockContext.Verify();
        }

        [Fact]
        public async Task GivenBookingStarted_WhenNotifyingTheSeller_ShouldSendAnEmailToTheSeller()
        {
            // given
            var bookingStarted = new BookingStarted();

            // mock email
            var mockEmailSender = new Mock<ISendEmails>();
            mockEmailSender.Setup(_ => _.SendAsync("sender@seller.com", "seller@seller.com", "New purchase order", "A new purchase order is available for approval")).Verifiable();

            //when
            var reaction = new SendNotificationMail(new Mock<ILogger<SendNotificationMail>>().Object, mockEmailSender.Object);
            await reaction.Handle(bookingStarted, null);

            // Then
            mockEmailSender.Verify();
        }

    }

}