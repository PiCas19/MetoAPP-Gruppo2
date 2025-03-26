using Newtonsoft.Json;
using System.Globalization;
using MeteoAPP.Models;

namespace MeteoAPP.Services
{
    public class OpenWeatherService
    {
        private readonly HttpClient _httpClient;
        private const string API_KEY = "e67488d5bc369d848639ee03d0b0e5ac";

        public OpenWeatherService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<WeatherData?> GetWeatherByCoordinatesAsync(double latitude, double longitude)
        {
            try
            {
                var url = $"https://api.openweathermap.org/data/2.5/weather?lat={latitude}&lon={longitude}&appid={API_KEY}&units=metric";
                
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                
                var json = await response.Content.ReadAsStringAsync();
                var weatherResponse = JsonConvert.DeserializeObject<WeatherResponse>(json);

                if (weatherResponse == null)
                {
                    System.Diagnostics.Debug.WriteLine("Errore: Nessun dato meteo ricevuto");
                    return null;
                }

                return new WeatherData
                {
                    Location = weatherResponse.Name ?? "N/A",
                    Description = weatherResponse.Weather?[0]?.Description ?? "N/A",
                    IconCode = weatherResponse.Weather?[0]?.Icon ?? "N/A",
                    Temperature = weatherResponse.Main?.Temp ?? 0,
                    TemperatureMin = weatherResponse.Main?.TempMin ?? 0,
                    TemperatureMax = weatherResponse.Main?.TempMax ?? 0
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore in GetWeatherByCoordinatesAsync: {ex.Message}");
                return null;
            }
        }
    }
}