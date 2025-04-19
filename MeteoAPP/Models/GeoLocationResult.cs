namespace MeteoAPP.Models
{

    /// <summary>
    /// Rappresenta il risultato di una richiesta di geolocalizzazione.
    /// Contiene informazioni sulla posizione ottenuta e sull'esito dell'operazione.
    /// </summary>
    public class GeoLocationResult
    {
        /// <summary>
        /// Indica se la richiesta di geolocalizzazione è andata a buon fine.
        /// </summary>
        public bool Success { get; set; }
        /// <summary>
        /// Latitudine della posizione rilevata.
        /// </summary>
        public double Latitude { get; set; } 
        /// <summary>
        /// Longitudine della posizione rilevata.
        /// </summary>
        public double Longitude { get; set; }
        /// <summary>
        /// Messaggio di errore in caso di fallimento della richiesta.
        /// </summary>
        public string ErrorMessage { get; set; } = string.Empty;
    }
}
