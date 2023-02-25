using MessageHandler.EventSourcing.DomainModel;
using Microsoft.AspNetCore.Mvc;

namespace OrderBooking.WebAPI.Controllers
{
    [ApiController]
    [Route("api/notificationpreferences")]
    public class NotificationPreferencesController : ControllerBase
    {
        private IEventSourcedRepository<NotificationPreferences.NotificationPreferences> repository;

        public NotificationPreferencesController(IEventSourcedRepository<NotificationPreferences.NotificationPreferences> repository)
        {
            this.repository = repository;
        }

        [HttpPost("{buyerId}")]
        public async Task<IActionResult> Book([FromRoute] string buyerId, [FromBody] SetConfirmationMail command)
        {
            var preferences = await repository.Get(buyerId);

            preferences.SetConfirmationEmail(command.EmailAddress);

            await repository.Flush();

            return Ok();
        }
    }
}
