using MeteoAPP.Models;

namespace MeteoAPP.Services {
    /// <summary>
    /// Interfaccia per il servizio di passaggio dati tra pagine.
    /// Utilizzato per condividere istanze di <see cref="WeatherData"/>.
    /// </summary>
    public interface IParameterService
    {
        /// <summary>
        /// Restituisce l'oggetto <see cref="WeatherData"/> attualmente salvato.
        /// </summary>
        /// <returns>Oggetto contenente i dati meteo.</returns>
        WeatherData GetData();
        /// <summary>
        /// Imposta un nuovo oggetto <see cref="WeatherData"/> da condividere.
        /// </summary>
        /// <param name="newData">Istanza di <see cref="WeatherData"/> da salvare.</param>
        void SetData(WeatherData newData);
    }

    /// <summary>
    /// Implementazione concreta di <see cref="IParameterService"/>.
    /// Permette il salvataggio temporaneo dei dati meteo tra componenti.
    /// </summary>
    public class ParameterService : IParameterService
    {
        private WeatherData _data = new WeatherData();

        /// <inheritdoc/>
        public WeatherData GetData()
        {
            return _data;
        }
        /// <inheritdoc/>
        public void SetData(WeatherData newData)
        {
            _data = newData;
        }
    }
}
