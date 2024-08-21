using MessageHandler.EventSourcing.DomainModel;
using OrderAggregate = OrderBooking.OrderBooking;
using OrderBooking.WebAPI.Controllers;
using MessageHandler.EventSourcing.Projections;
using OrderBooking.Projections;

namespace OrderBooking.WebAPI;

public static class MinimalApiConfig
{
    public static RouteGroupBuilder UseOrderBooking(
        this IEndpointRouteBuilder builder,
        Func<IEndpointRouteBuilder, RouteGroupBuilder> groupBuilder)
    {
        var orderBookings = groupBuilder(builder);

        orderBookings.MapPost("{id}",
        async (IEventSourcedRepository<OrderAggregate> repo, string id, PlacePurchaseOrder cmd) =>
        {
            var booking = await repo.Get(id);
            booking.PlacePurchaseOrder(cmd.PurchaseOrder, cmd.Name);

            await repo.Flush();

            return Results.Created($"api/orderbooking/{id}", booking.Id);
        })
        .Produces(StatusCodes.Status201Created);;

        orderBookings.MapGet("{id}", async(IRestoreProjections<Booking> projector, string id) =>
            Results.Ok(await projector.Restore(nameof(OrderAggregate), id))
        ).Produces(StatusCodes.Status200OK);

        return orderBookings;
    }
}