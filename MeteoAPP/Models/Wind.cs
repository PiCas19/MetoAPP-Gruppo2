using Newtonsoft.Json;

namespace MeteoAPP.Models
{
    /// <summary>
    /// Rappresenta i dati relativi al vento, inclusa la velocità e la direzione.
    /// Queste informazioni provengono dalla risposta dell'API OpenWeather.
    /// </summary>
    public class Wind
    {
        /// <summary>
        /// Velocità del vento in metri al secondo (m/s).
        /// </summary>
        [JsonProperty("speed")]
        public double Speed { get; set; }
        /// <summary>
        /// Direzione del vento in gradi (0° = Nord, 90° = Est, 180° = Sud, 270° = Ovest).
        /// </summary>
        [JsonProperty("deg")]
        public int Deg { get; set; }
    }
}
