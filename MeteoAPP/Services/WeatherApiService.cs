using Newtonsoft.Json;
using MeteoAPP.Models;

namespace MeteoAPP.Services
{
    /// <summary>
    /// Servizio per il recupero dei dati meteo da WeatherAPI.
    /// </summary>
    public class WeatherApiService : IWeatherService
    {
        private readonly HttpClient _httpClient;
        private string _apiKey = "";

        public WeatherApiService()
        {
            _httpClient = new HttpClient();
        }

        public async Task InitializeAsync()
        {
            await ConfigService.Instance.InitializeAsync();
            _apiKey = ConfigService.Instance.GetWeatherApiKey() ?? "";

            if (string.IsNullOrEmpty(_apiKey))
                throw new InvalidOperationException("WeatherAPI Key non trovata nel config.json");
        }

        public async Task<WeatherData?> GetWeatherByCoordinatesAsync(double latitude, double longitude)
        {
            try
            {
                var url = $"https://api.weatherapi.com/v1/forecast.json?key={_apiKey}&q={latitude},{longitude}&days=1&aqi=no&alerts=no";
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var weatherResponse = JsonConvert.DeserializeObject<WeatherSecondResponse>(json);

                if (weatherResponse == null)
                    return null;

                // Prova a ottenere i dati forecast, altrimenti fallback a current
                var forecastDay = weatherResponse.Forecast?.ForecastDays?.FirstOrDefault();
                var hasForecast = forecastDay != null;

                return new WeatherData
                {
                    Location = weatherResponse.Location?.Name ?? "N/A",
                    Description = weatherResponse.Current?.Condition?.Text ?? "N/A",
                    IconCode = weatherResponse.Current?.Condition?.Icon ?? "01d",
                    Temperature = weatherResponse.Current?.TempC ?? 0,
                    TemperatureMin = hasForecast ? forecastDay!.Day?.MinTempC ?? 0 : weatherResponse.Current?.TempC ?? 0,
                    TemperatureMax = hasForecast ? forecastDay!.Day?.MaxTempC ?? 0 : weatherResponse.Current?.TempC ?? 0,
                    WindSpeedKmh = weatherResponse.Current?.WindKph ?? 0,
                    RainChancePercent = hasForecast ? forecastDay!.Day?.DailyChanceOfRain ?? 0 : 0,
                    PressureHpa = weatherResponse.Current?.PressureMb ?? 0,

                    // Ore del giorno: se forecast non esiste, fallback a current temp
                    MorningTemp = hasForecast ? forecastDay!.Hours?.FirstOrDefault(h => h.Time.EndsWith("06:00"))?.TempC ?? 0 : weatherResponse.Current?.TempC ?? 0,
                    AfternoonTemp = hasForecast ? forecastDay!.Hours?.FirstOrDefault(h => h.Time.EndsWith("15:00"))?.TempC ?? 0 : weatherResponse.Current?.TempC ?? 0,
                    EveningTemp = hasForecast ? forecastDay!.Hours?.FirstOrDefault(h => h.Time.EndsWith("18:00"))?.TempC ?? 0 : weatherResponse.Current?.TempC ?? 0,
                    NightTemp = hasForecast ? forecastDay!.Hours?.FirstOrDefault(h => h.Time.EndsWith("21:00"))?.TempC ?? 0 : weatherResponse.Current?.TempC ?? 0
                };
            }
            catch (Exception ex)
            {
                Android.Util.Log.Debug("WeatherApiService", $"Errore in GetWeatherByCoordinatesAsync: {ex.Message}");
                return null;
            }
        }
    }
}
