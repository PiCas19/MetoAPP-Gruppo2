namespace MeteoAPP.Models
{
    /// <summary>
    /// Rappresenta una località geografica con nome, latitudine e longitudine.
    /// Utilizzato per gestire le coordinate delle città selezionate.
    /// </summary>
    public class GeoLocation
    {
        /// <summary>
        /// Nome della località (es. città).
        /// </summary>
        public string? Name { get; set; }
        /// <summary>
        /// Latitudine geografica della località.
        /// </summary>
        public double Latitude { get; set; }
        /// <summary>
        /// Longitudine geografica della località.
        /// </summary>
        public double Longitude { get; set; }
        }

}