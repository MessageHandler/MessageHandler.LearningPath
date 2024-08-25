using MessageHandler.EventSourcing.Contracts;
using Microsoft.AspNetCore.SignalR;
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
        private readonly Mock<IClientProxy> mockGroups = new();
        private readonly Mock<IHubClients> mockClients = new();
        private readonly Mock<IHubContext<EventsHub>> mockContext = new();
        private readonly Mock<ISendEmails> mockEmailSender = new();
        public WhenReactingToBookingStarted()
        {
            mockGroups.Setup(x => x.SendCoreAsync("Notify", new object?[0], It.IsAny<CancellationToken>()));
            mockClients.Setup(x => x.Group("all")).Returns(mockGroups.Object).Verifiable();
            mockContext.Setup(x => x.Clients).Returns(mockClients.Object).Verifiable();
            mockEmailSender.Setup(x => x.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask).Verifiable();
        }
        [Fact]
        public async Task GivenBookingStarted_WhenNotifyingTheSeller_ShouldForwardMessageToSignal()
        {
            // given
            var bookingStarted = new BookingStarted("", "", "", new PurchaseOrder(1));
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
            var bookingStarted = new BookingStarted("", "", "", new PurchaseOrder(1));
            
            //when
            var reaction = new SendNotificationMail(mockEmailSender.Object);
            await reaction.Handle(bookingStarted, null!);

            // Then
            mockEmailSender.Verify();
        }

    }

}