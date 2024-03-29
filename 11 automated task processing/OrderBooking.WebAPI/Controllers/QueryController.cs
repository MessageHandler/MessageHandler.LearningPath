﻿using Azure.Search.Documents;
using MessageHandler.EventSourcing.Projections;
using Microsoft.AspNetCore.Mvc;
using OrderBooking.Projections;

namespace OrderBooking.WebAPI.Controllers
{
    [Route("api/orderbooking")]
    [ApiController]
    public class QueryController : ControllerBase
    {
        private IRestoreProjections<Booking> projection;
        private SearchClient _search;

        public QueryController(IRestoreProjections<Booking> projection, SearchClient search)
        {
            this.projection = projection;
            _search = search;
        }

        [HttpGet("{bookingId}")]
        public async Task<IActionResult> Get([FromRoute] string bookingId)
        {
            return Ok(await projection.Restore(nameof(OrderBooking), bookingId));
        }

        [HttpGet("pending")]
        public async Task<IActionResult> Pending()
        {
            var filter = "Status eq 'Pending'";

            var response = await _search.SearchAsync<SalesOrder>("*", new SearchOptions() { Filter = filter, Size = 1000 });

            var pendingOrders = response.Value.GetResults().Select(r => r.Document);

            return Ok(pendingOrders);
        }
    }
}
