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
                var url = $"https://api.weatherapi.com/v1/current.json?key={_apiKey}&q={latitude},{longitude}&aqi=no";
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var weatherResponse = JsonConvert.DeserializeObject<WeatherSecondResponse>(json);

                if (weatherResponse == null)
                    return null;

                return new WeatherData
                {
                    Location = "Bella",
                    Description = weatherResponse.Current?.Condition?.Text ?? "N/A",
                    IconCode = weatherResponse.Current?.Condition?.Icon ?? "01d",
                    Temperature = weatherResponse.Current?.TempC ?? 0,
                    TemperatureMin = weatherResponse.Current?.TempC ?? 0,
                    TemperatureMax = weatherResponse.Current?.TempC ?? 0,
                    WindSpeedKmh = weatherResponse.Current?.WindKph ?? 0,
                    RainChancePercent = weatherResponse.Current?.Cloud ?? 0,
                    PressureHpa = weatherResponse.Current?.PressureMb ?? 0,
                    MorningTemp = weatherResponse.Current?.TempC ?? 0,
                    AfternoonTemp = weatherResponse.Current?.TempC ?? 0,
                    EveningTemp = weatherResponse.Current?.TempC ?? 0,
                    NightTemp = weatherResponse.Current?.TempC ?? 0
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
