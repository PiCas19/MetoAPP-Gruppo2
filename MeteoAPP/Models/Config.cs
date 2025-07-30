
namespace MeteoAPP.Models
{
    /// <summary>
    /// Modello di configurazione contenente le chiavi e gli identificativi per l'integrazione con servizi esterni.
    /// Caricato da un file JSON (es. config.json).
    /// </summary>
    public class Config
    {
        /// <summary>
        /// API key per accedere al servizio di previsioni meteo OpenWeather.
        /// </summary>
        public string? OpenWeatherApiKey { get; set; }
        /// <summary>
        /// API key per accedere al servizio di previsioni meteo WeatherApi.
        /// </summary>
        public string? WeatherApiKey { get; set; }
        /// <summary>
        /// Identificativo del progetto Appwrite.
        /// </summary>
        public string? AppwriteProjectId { get; set; }
        /// <summary>
        /// Chiave API privata per l'autenticazione con Appwrite.
        /// </summary>
        public string? AppwriteApiKey { get; set; }
        /// <summary>
        /// Identificativo del database Appwrite.
        /// </summary>
        public string? AppwriteDatabaseId { get; set; }
        /// <summary>
        /// Identificativo della collezione Appwrite in cui vengono salvati i dati.
        /// </summary>
        public string? AppwriteCollectionId { get; set; }
        /// <summary>
        /// URL base dell'API per la gestione delle impostazioni di notifica personalizzate.
        /// </summary>
        public string? NotificationSettingsBaseUrl { get; set; }
    }
}