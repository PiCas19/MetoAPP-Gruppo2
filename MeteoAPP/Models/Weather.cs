using Newtonsoft.Json;
namespace MeteoAPP.Models
{
    /// <summary>
    /// Rappresenta una descrizione meteo di una località,
    /// inclusa la descrizione testuale e l'icona associata.
    /// </summary>
    public class Weather
    {
        /// <summary>
        /// Descrizione testuale delle condizioni meteorologiche
        /// (es. "clear sky", "light rain").
        /// </summary>
        [JsonProperty("description")]
        public string? Description { get; set; }

        /// <summary>
        /// Codice identificativo dell'icona meteo fornita dall'API OpenWeather.
        /// Può essere utilizzato per generare un URL di immagine.
        /// </summary>
        [JsonProperty("icon")]
        public string? Icon { get; set; }
    }
}