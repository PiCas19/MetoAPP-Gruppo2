using System.Collections.Generic;

namespace BackendProject
{
    public static class NotificationSettingsRepository
    {
        // In un'app reale, sostituisci questa lista con un database.
        public static List<UserNotificationSettings> Settings { get; } = new List<UserNotificationSettings>();
    }
}