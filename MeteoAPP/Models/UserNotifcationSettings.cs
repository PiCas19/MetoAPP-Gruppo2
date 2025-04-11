namespace MeteoAPP.Models
{

    public class UserNotificationSettings
    {
        public string? Token { get; set; }
        public string? Location { get; set; }
        public double TemperatureMax { get; set; }
        public double TemperatureMin { get; set; }
    }
}