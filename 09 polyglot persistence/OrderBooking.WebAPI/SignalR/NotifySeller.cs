using MessageHandler.EventSourcing.Contracts;
using Microsoft.AspNetCore.SignalR;

namespace OrderBooking.WebAPI.SignalR
{
    public class NotifySeller : MessageHandler.EventSourcing.DomainModel.IChannel
    {
        private readonly IHubContext<EventsHub> _context;

        public NotifySeller(IHubContext<EventsHub> context)
        {
            _context = context;
        }

        public Task Push(IEnumerable<SourcedEvent> events)
        {
            var tasks = new List<Task>();
            foreach (var e in events)
            {
                tasks.Add(_context.Clients.Group("all").SendAsync("Notify", e));
            }            
            return Task.WhenAll(tasks);
        }
    }
}
