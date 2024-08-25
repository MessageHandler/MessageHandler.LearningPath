using MessageHandler.EventSourcing.DomainModel;
using Booking = OrderBooking.OrderBooking;
using OrderBooking.WebAPI.Controllers;
using MessageHandler.EventSourcing.Projections;
using OrderBooking.Projections;

namespace OrderBooking.WebAPI;

public static class ApiRoutingConfiguration
{
    public static RouteGroupBuilder UseOrderBooking(
        this IEndpointRouteBuilder builder,
        Func<IEndpointRouteBuilder, RouteGroupBuilder> groupBuilder)
    {
        var orderBookings = groupBuilder(builder);

        orderBookings.MapPost("{bookingId}",
        async (IEventSourcedRepository<Booking> repo, string bookingId, PlacePurchaseOrder cmd) =>
        {
            var booking = await repo.Get(bookingId);
            booking.PlacePurchaseOrder(cmd.PurchaseOrder, cmd.Name);

            await repo.Flush();

            return Results.Created($"api/orderbooking/{bookingId}", booking.Id);
        })
        .Produces(StatusCodes.Status201Created);

        orderBookings.MapGet("{bookingId}", async(IRestoreProjections<Booking> projector, string bookingId) =>
            Results.Ok(await projector.Restore(nameof(Booking), bookingId))
        ).Produces(StatusCodes.Status200OK);

        return orderBookings;
    }
}