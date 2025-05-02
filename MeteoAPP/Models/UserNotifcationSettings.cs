using Newtonsoft.Json;
using System.Text.Json.Serialization;


namespace MeteoAPP.Models
{
    /// <summary>
    /// Rappresenta le impostazioni di notifica personalizzate dell'utente 
    /// per una determinata località.
    /// </summary>
    public class UserNotificationSettings
    {
        /// <summary>
        /// Token univoco del dispositivo Firebase utilizzato per inviare notifiche push.
        /// </summary>
        [JsonPropertyName("token")]
        public string? Token { get; set; }
        /// <summary>
        /// Nome della località associata alle impostazioni di notifica.
        /// </summary>
        [JsonPropertyName("location")]
        public string? Location { get; set; }
        /// <summary>
        /// Soglia massima di temperatura oltre la quale viene inviata una notifica.
        /// </summary>
        [JsonPropertyName("temperatureMax")]
        public double TemperatureMax { get; set; }
        /// <summary>
        /// Soglia minima di temperatura sotto la quale viene inviata una notifica.
        /// </summary>
        [JsonPropertyName("temperatureMin")]
        public double TemperatureMin { get; set; }
        /// <summary>
        /// Flag che indica se le notifiche sono attive o disattivate per l'utente.
        /// </summary>
        [JsonPropertyName("isEnabled")]
        public bool IsEnabled { get; set; }

    }
}