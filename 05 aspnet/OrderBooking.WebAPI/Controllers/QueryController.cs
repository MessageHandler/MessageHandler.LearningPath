using MessageHandler.EventSourcing.Projections;
using Microsoft.AspNetCore.Mvc;
using OrderBooking.Projections;

namespace OrderBooking.WebAPI.Controllers
{
    [Route("api/orderbooking")]
    [ApiController]
    public class QueryController(IRestoreProjections<Booking> proj) : ControllerBase
    {
        private IRestoreProjections<Booking> projection = proj;

        [HttpGet("{bookingId}")]
        public async Task<IActionResult> Get([FromRoute] string bookingId)
        {
            return Ok(await projection.Restore(nameof(OrderBooking), bookingId));
        }
    }
}
