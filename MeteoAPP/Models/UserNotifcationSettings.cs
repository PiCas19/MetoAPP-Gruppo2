using Newtonsoft.Json;
using System.Text.Json.Serialization;


namespace MeteoAPP.Models
{

    public class UserNotificationSettings
    {
        [JsonPropertyName("token")]
        public string? Token { get; set; }
        
        [JsonPropertyName("location")]
        public string? Location { get; set; }
        
        [JsonPropertyName("temperatureMax")]
        public double TemperatureMax { get; set; }
        
        [JsonPropertyName("temperatureMin")]
        public double TemperatureMin { get; set; }

        [JsonPropertyName("isEnabled")]
        public bool IsEnabled { get; set; }

    }
}