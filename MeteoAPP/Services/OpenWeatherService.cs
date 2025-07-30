using Newtonsoft.Json;
using MeteoAPP.Models;

namespace MeteoAPP.Services
{
    /// <summary>
    /// Servizio per il recupero dei dati meteo da OpenWeather API.
    /// Gestisce la configurazione dell'API key e le richieste HTTP verso l'endpoint meteo.
    /// </summary>
    public class OpenWeatherService : IWeatherService
    {
        private readonly HttpClient _httpClient;
        private string _apiKey = "";

        /// <summary>
        /// Costruttore della classe <see cref="OpenWeatherService"/>.
        /// Inizializza l'istanza di HttpClient.
        /// </summary>
        public OpenWeatherService()
        {
            _httpClient = new HttpClient();
        }

        /// <inheritdoc />
        public async Task InitializeAsync()
        {
            await ConfigService.Instance.InitializeAsync();
            _apiKey = ConfigService.Instance.GetOpenWeatherApiKey() ?? "";

            if (string.IsNullOrEmpty(_apiKey))
                throw new InvalidOperationException("API Key non trovata nel config.json");
        }

        /// <inheritdoc />
        public async Task<WeatherData?> GetWeatherByCoordinatesAsync(double latitude, double longitude)
        {
            try
            {
                var url = $"https://api.openweathermap.org/data/2.5/weather?lat={latitude}&lon={longitude}&appid={_apiKey}&units=metric";
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var weatherResponse = JsonConvert.DeserializeObject<WeatherResponse>(json);

                return weatherResponse == null ? null : new WeatherData
                {
                    Location = weatherResponse.Name ?? "N/A",
                    Description = weatherResponse.Weather?[0]?.Description ?? "N/A",
                    IconCode = weatherResponse.Weather?[0]?.Icon ?? "N/A",
                    Temperature = weatherResponse.Main?.Temp ?? 0,
                    TemperatureMin = weatherResponse.Main?.TempMin ?? 0,
                    TemperatureMax = weatherResponse.Main?.TempMax ?? 0,
                    WindSpeedKmh = (weatherResponse.Wind?.Speed ?? 0) * 3.6,
                    RainChancePercent = weatherResponse.Clouds?.All ?? 0,
                    PressureHpa = weatherResponse.Main?.Pressure ?? 0,
                    MorningTemp = (weatherResponse.Main?.TempMin ?? 0) + 0.5,
                    AfternoonTemp = weatherResponse.Main?.Temp ?? 0,
                    EveningTemp = (weatherResponse.Main?.TempMax ?? 0) - 0.5,
                    NightTemp = weatherResponse.Main?.TempMin ?? 0
                };
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
