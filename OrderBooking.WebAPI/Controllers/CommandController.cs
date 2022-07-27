using MessageHandler.EventSourcing.DomainModel;
using Microsoft.AspNetCore.Mvc;

namespace OrderBooking.WebAPI.Controllers
{
    [ApiController]
    [Route("api/orderbooking")]
    public class CommandController : ControllerBase
    {
        private IEventSourcedRepository repository;

        public CommandController(IEventSourcedRepository repository)
        {
            this.repository = repository;
        }

        [HttpPost("{bookingId}")]
        public async Task<IActionResult> Book([FromRoute] string bookingId, [FromBody] PlacePurchaseOrder command)
        {
            var booking = await repository.Get<OrderBooking>(bookingId);

            booking.PlacePurchaseOrder(command.PurchaseOrder);

            await repository.Flush();

            return Ok();
        }
    }
}
