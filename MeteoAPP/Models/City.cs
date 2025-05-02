using SQLite;

namespace MeteoAPP.Models
{
    /// <summary>
    /// Rappresenta una città memorizzata nel database locale.
    /// Contiene informazioni geografiche come nome, nazione, latitudine e longitudine.
    /// </summary>
    public class City
    {
        /// <summary>
        /// Identificatore univoco della città nel database (autoincrementale).
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public long Id { get; set; }

        /// <summary>
        /// Longitudine della posizione della città.
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// Latitudine della posizione della città.
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// Nome della città.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Nome del paese a cui appartiene la città.
        /// </summary>
        public string? Country { get; set; }
    }
}
