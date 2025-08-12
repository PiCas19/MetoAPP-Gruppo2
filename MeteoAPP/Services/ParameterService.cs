using MeteoAPP.Models;

namespace MeteoAPP.Services
{
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

        /// <summary>
        /// Restituisce gli ultimi 7 giorni di storico meteo per la città specificata.
        /// </summary>
        /// <param name="cityId">ID della città</param>
        /// <returns>Lista di record meteo</returns>
        Task<List<WeatherHistory>> GetLast7DaysHistoryAsync(string cityId);

    }

    /// <summary>
    /// Implementazione concreta di <see cref="IParameterService"/>.
    /// Permette il salvataggio temporaneo dei dati meteo tra componenti.
    /// </summary>
    public class ParameterService : IParameterService
    {
        private WeatherData _data = new WeatherData();
        private readonly DatabaseService _databaseService;

        public ParameterService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public WeatherData GetData() => _data;

        public void SetData(WeatherData newData) => _data = newData;

        public async Task<List<WeatherHistory>> GetLast7DaysHistoryAsync(string cityId)
        {
            var now = DateTime.UtcNow.Date;
            var weekAgo = now.AddDays(-6); // 7 giorni compresi oggi

            var all = await _databaseService.GetLast7DaysWeatherByCityAsync(cityId);
            return all
                .Where(h => h.Date.Date >= weekAgo && h.Date.Date <= now)
                .OrderBy(h => h.Date)
                .ToList();
        }
    }

}
