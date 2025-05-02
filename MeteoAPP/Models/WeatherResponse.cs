using Newtonsoft.Json;
namespace MeteoAPP.Models 
{
    /// <summary>
    /// Modello che rappresenta la risposta JSON del servizio OpenWeather API
    /// contenente informazioni meteo generali per una località.
    /// </summary>
    public class WeatherResponse
    {

        /// <summary>
        /// Nome della località per cui sono stati recuperati i dati meteo.
        /// </summary>
        [JsonProperty("name")]
        public string? Name { get; set; }
        /// <summary>
        /// Array di oggetti che descrivono le condizioni atmosferiche (es. pioggia, sole).
        /// </summary>
        [JsonProperty("weather")]
        public Weather[]? Weather { get; set; }
        /// <summary>
        /// Oggetto che rappresenta le informazioni termiche e di pressione.
        /// </summary>
        [JsonProperty("main")]
        public Main? Main { get; set; }
        /// <summary>
        /// Oggetto che rappresenta i dati relativi alla velocità e direzione del vento.
        /// </summary>
        [JsonProperty("wind")]
        public Wind? Wind { get; set; }
        /// <summary>
        /// Oggetto che rappresenta la copertura nuvolosa espressa in percentuale.
        /// </summary>
        [JsonProperty("clouds")]
        public Clouds? Clouds { get; set; }
    }
}