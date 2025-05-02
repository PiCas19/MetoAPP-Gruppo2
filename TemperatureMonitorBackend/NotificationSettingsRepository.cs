using System.Collections.Generic;

namespace BackendProject
{
    /// <summary>
    /// Repository in memoria per la memorizzazione temporanea delle impostazioni utente.
    /// Utilizzata per test o fallback, non persistente.
    /// </summary>
    public static class NotificationSettingsRepository
    {
        /// <summary>
        /// Collezione statica che contiene le impostazioni di notifica degli utenti.
        /// </summary>
        public static List<UserNotificationSettings> Settings { get; } = new List<UserNotificationSettings>();
    }
}