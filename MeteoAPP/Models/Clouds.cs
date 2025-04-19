using Newtonsoft.Json;

namespace MeteoAPP.Models
{
    public class Clouds
    {
        [JsonProperty("all")]
        public int All { get; set; }  
    }
}
