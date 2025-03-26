using Newtonsoft.Json;
namespace MeteoAPP.Models 
{
    public class WeatherResponse
    {
        [JsonProperty("name")]
        public string? Name { get; set; }

        [JsonProperty("weather")]
        public Weather[]? Weather { get; set; }

        [JsonProperty("main")]
        public Main? Main { get; set; }
    }
}