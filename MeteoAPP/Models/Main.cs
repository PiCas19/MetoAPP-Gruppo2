using Newtonsoft.Json;
namespace MeteoAPP.Models
{
    /// <summary>
    /// Rappresenta i dati principali del meteo restituiti dall'API OpenWeather.
    /// </summary>
    public class Main
    {
        /// <summary>
        /// Temperatura corrente in gradi Celsius.
        /// </summary>
        [JsonProperty("temp")]
        public double Temp { get; set; }
        /// <summary>
        /// Temperatura minima prevista.
        /// </summary>
        [JsonProperty("temp_min")]
        public double TempMin { get; set; }
        /// <summary>
        /// Temperatura massima prevista.
        /// </summary>
        [JsonProperty("temp_max")]
        public double TempMax { get; set; }
        /// <summary>
        /// Pressione atmosferica in hPa.
        /// </summary>
        [JsonProperty("pressure")]
        public double Pressure { get; set; }
    }
}