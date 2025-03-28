using Newtonsoft.Json;
using MeteoAPP.Models;

namespace MeteoAPP.Services
{
    public class OpenWeatherService
    {
        private readonly HttpClient _httpClient;
        private string _apiKey;

        public OpenWeatherService()
        {
            _httpClient = new HttpClient();
            _apiKey = string.Empty;
        }

        public async Task InitializeAsync()
        {
            _apiKey = await LoadApiKeyFromConfigAsync();
            if (string.IsNullOrEmpty(_apiKey))
            {
                throw new InvalidOperationException("The API key was not found in the configuration file.");
            }
        }

        private async Task<string> LoadApiKeyFromConfigAsync()
        {
            try
            {
                var assembly = GetType().Assembly;
                var resourceName = assembly.GetManifestResourceNames()
                    .FirstOrDefault(rn => rn.EndsWith("config.json"));

                if (resourceName != null)
                {
                    using var stream = assembly.GetManifestResourceStream(resourceName);
                    if (stream != null)
                    {
                        using var reader = new StreamReader(stream);
                        var jsonContent = await reader.ReadToEndAsync();
                        var config = JsonConvert.DeserializeObject<Config>(jsonContent);
                        return config?.OpenWeatherApiKey ?? string.Empty;
                    }
                }
                var configFilePath = Path.Combine(FileSystem.AppDataDirectory, "config.json");
                
                var directory = Path.GetDirectoryName(configFilePath);
                if (!string.IsNullOrEmpty(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                if (!File.Exists(configFilePath))
                {
                    await File.WriteAllTextAsync(configFilePath, JsonConvert.SerializeObject(new Config()));
                }

                var fileContent = await File.ReadAllTextAsync(configFilePath);
                var fileConfig = JsonConvert.DeserializeObject<Config>(fileContent);
                
                return fileConfig?.OpenWeatherApiKey ?? string.Empty;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading API key: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return string.Empty;
            }
        }
        
        public async Task<WeatherData?> GetWeatherByCoordinatesAsync(double latitude, double longitude)
        {
            try
            {
                var url = $"https://api.openweathermap.org/data/2.5/weather?lat={latitude}&lon={longitude}&appid={_apiKey}&units=metric";
                
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                
                var json = await response.Content.ReadAsStringAsync();
                var weatherResponse = JsonConvert.DeserializeObject<WeatherResponse>(json);

                if (weatherResponse == null)
                {
                    System.Diagnostics.Debug.WriteLine("Error: No weather data received.");
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
                System.Diagnostics.Debug.WriteLine($"Error in GetWeatherByCoordinatesAsync: {ex.Message}");
                return null;
            }
        }
    }
}