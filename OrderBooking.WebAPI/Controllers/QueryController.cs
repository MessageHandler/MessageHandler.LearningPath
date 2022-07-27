using MessageHandler.EventSourcing.Projections;
using Microsoft.AspNetCore.Mvc;
using OrderBooking.Projections;

namespace OrderBooking.WebAPI.Controllers
{
    [Route("api/orderbooking")]
    [ApiController]
    public class QueryController : ControllerBase
    {
        private IRestoreProjections projection;

        public QueryController(IRestoreProjections projection)
        {
            this.projection = projection;   
        }

        [HttpGet("{bookingId}")]
        public async Task<IActionResult> Get([FromRoute] string bookingId)
        {
            return Ok(await projection.Restore<Booking>(nameof(OrderBooking), bookingId));
        }
    }
}
