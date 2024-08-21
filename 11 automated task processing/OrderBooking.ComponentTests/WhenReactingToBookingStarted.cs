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
        private readonly Mock<IClientProxy> mockClientProxy = new();
        private readonly Mock<IHubClients> mockHubClients = new();
        private readonly Mock<IHubContext<EventsHub>> mockHubContext = new();
        private readonly Mock<IEmailService> mockEmailService = new();
        public ReactingToBookingStarted()
        {
            mockClientProxy.Setup(x => x.SendCoreAsync("Notify", new object?[0], It.IsAny<CancellationToken>()));
            mockHubClients.Setup(x => x.Group("all")).Returns(mockClientProxy.Object).Verifiable();
            mockHubContext.Setup(x => x.Clients).Returns(mockHubClients.Object).Verifiable();
            mockEmailService.Setup(x => x.SendAsync(It.IsAny<string>())).Returns(Task.CompletedTask).Verifiable();
        }
        [Fact]
        public async Task GivenBookingStarted_WhenNotifyingTheSeller_ShouldForwardMessageToSignal()
        {
            // given
            var bookingStarted = new BookingStarted();
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
            
            //when
            var reaction = new SendNotificationMail(mockEmailSender.Object);
            await reaction.Handle(bookingStarted, null!);

            // Then
            mockEmailSender.Verify();
        }

    }

}