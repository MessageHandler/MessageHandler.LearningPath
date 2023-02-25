namespace OrderBooking.Worker
{
    public interface IPersistNotificationPreferences
    {
        public Task<NotificationPreferences?> GetNotificationPreferences(string id);

        public Task Insert(NotificationPreferences preferences);

        public Task Update(NotificationPreferences preferences);
    }
}
