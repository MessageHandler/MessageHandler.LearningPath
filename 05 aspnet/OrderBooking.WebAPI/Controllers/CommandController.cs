using MessageHandler.EventSourcing.DomainModel;
using Microsoft.AspNetCore.Mvc;

namespace OrderBooking.WebAPI.Controllers
{
    [ApiController]
    [Route("api/orderbooking")]
    public class CommandController(IEventSourcedRepository<OrderBooking> repo) : ControllerBase
    {
        private IEventSourcedRepository<OrderBooking> repository = repo;

        [HttpPost("{bookingId}")]
        public async Task<IActionResult> Book([FromRoute] string bookingId, [FromBody] PlacePurchaseOrder command)
        {
            var booking = await repository.Get(bookingId);

            booking.PlacePurchaseOrder(command.PurchaseOrder);

            await repository.Flush();

            return Ok();
        }
    }
}
