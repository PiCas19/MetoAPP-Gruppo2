using SQLite;
using MeteoApp.Models;

namespace MeteoApp.Services
{
    public class WeatherDatabase
    {
        private readonly SQLiteAsyncConnection _database;

        public WeatherDatabase(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
        }

        public async Task InitializeAsync()
        {
            await _database.CreateTableAsync<WeatherData>();
        }

        public Task<List<WeatherData>> GetWeatherDataAsync()
        {
            return _database.Table<WeatherData>().ToListAsync();
        }

        public Task<int> SaveWeatherDataAsync(WeatherData weather)
        {
            return _database.InsertAsync(weather);
        }

        public Task<int> DeleteWeatherDataAsync(WeatherData weather)
        {
            return _database.DeleteAsync(weather);
        }
    }
}
