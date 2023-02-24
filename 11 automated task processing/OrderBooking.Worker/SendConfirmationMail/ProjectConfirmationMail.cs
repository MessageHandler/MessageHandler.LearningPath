using MessageHandler.EventSourcing.Projections;
using OrderBooking.Events;

namespace OrderBooking.Worker
{
    public class ProjectConfirmationMail :
        IProjection<ConfirmationMail, BookingStarted>,
        IProjection<ConfirmationMail, SalesOrderConfirmed>
    {
        public void Project(ConfirmationMail confirmationMail, BookingStarted msg)
        {
            confirmationMail.OrderId = msg.BookingId;
            confirmationMail.BuyerId = msg.BuyerId;
            confirmationMail.SenderEmailAddress = "sender@seller.com";
            confirmationMail.EmailSubject = "Your purchase order";
            confirmationMail.EmailBody = "You order has been confirmed";
            confirmationMail.Status = "Draft";
        }

        public void Project(ConfirmationMail confirmationMail, SalesOrderConfirmed msg)
        {          
            confirmationMail.Status = "Pending";
        }
    }
}
