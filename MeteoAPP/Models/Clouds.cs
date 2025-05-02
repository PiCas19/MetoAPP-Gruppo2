using Newtonsoft.Json;

namespace MeteoAPP.Models
{
    /// <summary>
    /// Rappresenta le informazioni sulla copertura nuvolosa ottenute dal servizio OpenWeatherMap.
    /// </summary>
    public class Clouds
    {
        /// <summary>
        /// Percentuale di copertura nuvolosa (da 0 a 100).
        /// </summary>
        [JsonProperty("all")]
        public int All { get; set; }  
    }
}
