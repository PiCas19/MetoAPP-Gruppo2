using Newtonsoft.Json;
namespace MeteoAPP.Models
{
    public class Weather
    {
        [JsonProperty("description")]
        public string? Description { get; set; }

        [JsonProperty("icon")]
        public string? Icon { get; set; }
    }
}