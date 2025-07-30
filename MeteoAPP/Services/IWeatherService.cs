using MeteoAPP.Models;

namespace MeteoAPP.Services
{
    public interface IWeatherService
    {
        Task InitializeAsync();
        Task<WeatherData?> GetWeatherByCoordinatesAsync(double latitude, double longitude);
    }
}
