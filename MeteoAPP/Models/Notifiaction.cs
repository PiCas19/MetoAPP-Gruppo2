namespace MeteoAPP.Models
{
    public class NotificationSettings
    {
        public double HighTemperatureThreshold { get; set; }
        public double LowTemperatureThreshold { get; set; }
        public bool IsNotificationEnabled { get; set; }
    }
}