using Java.Lang;
using SQLite;
using System;

namespace MeteoAPP.Models
{
    /// <summary>
    /// Rappresenta una voce dello storico meteo per una determinata città e data.
    /// Contiene temperatura minima, massima e una descrizione delle condizioni meteo.
    /// </summary>
    public class WeatherHistory
    {
        /// <summary>
        /// Identificatore univoco dello storico meteo (autoincrementale).
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public long Id { get; set; }

        /// <summary>
        /// Nome della città a cui si riferiscono i dati meteo.
        /// </summary>
        public string? CityName { get; set; }

        /// <summary>
        /// Data del rilevamento meteo (solo giorno).
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Temperatura minima registrata in quel giorno.
        /// </summary>
        public double TemperatureMin { get; set; }

        /// <summary>
        /// Temperatura massima registrata in quel giorno.
        /// </summary>
        public double TemperatureMax { get; set; }

        /// <summary>
        /// Descrizione delle condizioni meteo (es. "Parzialmente nuvoloso").
        /// </summary>
        public string? Description { get; set; }
    }
}
