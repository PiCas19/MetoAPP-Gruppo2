namespace MeteoAPP.Models
{
    /// <summary>
    /// Rappresenta i dati meteo completi per una località specifica.
    /// Include informazioni sulla temperatura, condizioni atmosferiche,
    /// vento, pressione, e variazioni giornaliere.
    /// </summary>
    public class WeatherData
    {
        /// <summary>
        /// Nome della località associata ai dati meteo.
        /// </summary>
        public string Location { get; set; } = "N/A";
        /// <summary>
        /// Descrizione testuale delle condizioni meteo (es. "clear sky").
        /// </summary>
        public string Description { get; set; } = "N/A";
        /// <summary>
        /// Codice dell'icona meteo per rappresentazione grafica.
        /// </summary>
        public string IconCode { get; set; } = "N/A";
        /// <summary>
        /// Temperatura attuale in gradi Celsius.
        /// </summary>
        public double Temperature { get; set; }
        /// <summary>
        /// Temperatura minima prevista in gradi Celsius.
        /// </summary>
        public double TemperatureMin { get; set; }
        /// <summary>
        /// Temperatura massima prevista in gradi Celsius.
        /// </summary>
        public double TemperatureMax { get; set; }
        /// <summary>
        /// Velocità del vento in chilometri orari.
        /// </summary>
        public double WindSpeedKmh { get; set; }  
        /// <summary>
        /// Probabilità di pioggia espressa in percentuale.
        /// </summary>
        public double RainChancePercent { get; set; } 
        /// <summary>
        /// Pressione atmosferica in hectoPascal (hPa).
        /// </summary> 
        public double PressureHpa { get; set; } 
        /// <summary>
        /// Temperatura stimata per il mattino.
        /// </summary>
        public double MorningTemp { get; set; }
         /// <summary>
        /// Temperatura stimata per il pomeriggio.
        /// </summary>
        public double AfternoonTemp { get; set; }
        /// <summary>
        /// Temperatura stimata per la sera.
        /// </summary>
        public double EveningTemp { get; set; }
        /// <summary>
        /// Temperatura stimata per la notte.
        /// </summary>
        public double NightTemp { get; set; }
    }
}