using MessageHandler.EventSourcing.DomainModel;
using Microsoft.AspNetCore.Mvc;

namespace OrderBooking.WebAPI.Controllers
{
    [ApiController]
    [Route("api/orderbooking")]
    public class CommandController : ControllerBase
    {
        private IEventSourcedRepository<OrderBooking> repository;

        public CommandController(IEventSourcedRepository<OrderBooking> repository)
        {
            this.repository = repository;
        }

        [HttpPost("{bookingId}")]
        public async Task<IActionResult> Book([FromRoute] string bookingId, [FromBody] PlacePurchaseOrder command)
        {
            var booking = await repository.Get(bookingId);

            booking.PlacePurchaseOrder(command.PurchaseOrder, command.BuyerId, command.Name);

            await repository.Flush();

            return Ok();
        }

        [HttpPut("{bookingId}/confirm")]
        public async Task<IActionResult> Confirm([FromRoute] string bookingId, [FromBody] ConfirmSalesOrder command)
        {
            var booking = await repository.Get(bookingId);

            booking.ConfirmSalesOrder();

            await repository.Flush();

            return Ok();
        }
    }
}
