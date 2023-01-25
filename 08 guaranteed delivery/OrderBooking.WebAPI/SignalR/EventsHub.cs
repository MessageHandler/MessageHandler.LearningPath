using Microsoft.AspNetCore.SignalR;

namespace OrderBooking.WebAPI.SignalR
{
    public class EventsHub : Hub
    {
        public async Task Subscribe()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "all");
        }
    }
}
