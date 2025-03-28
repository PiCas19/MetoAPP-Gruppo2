using SQLite;
using MeteoAPP.Models;
using System.Diagnostics;

namespace MeteoAPP.Services
{
    public class DatabaseService
    {
        private readonly SQLiteAsyncConnection _databaseConnection;
        private const string SeededKey = "DatabaseSeeded";
        private bool _isInitialized = false;

        public DatabaseService()
        {
            try 
            {
                var dbPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), 
                    "meteo.db3"
                );
                
                var directoryPath = Path.GetDirectoryName(dbPath);
                if (!Directory.Exists(directoryPath))
                {
                    try 
                    {
                        Directory.CreateDirectory(directoryPath!);
                    }
                    catch (Exception dirEx)
                    {
                        Debug.WriteLine($"Directory creation error: {dirEx.Message}");
                    }
                }

                _databaseConnection = new SQLiteAsyncConnection(dbPath);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Initial Error DatabaseService: {ex.Message}");
                throw;
            }
        }

        public async Task InitializeAsync()
        {
            if (!_isInitialized)
            {
                try
                {
                    await _databaseConnection.CreateTableAsync<City>();
                    bool isSeeded = Preferences.Get(SeededKey, false);
                    if (!isSeeded)
                    {
                        await SeedDatabaseAsync();
                        Preferences.Set(SeededKey, true);
                    }

                    var existingCities = await _databaseConnection.Table<City>().ToListAsync();
                    _isInitialized = true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error during database initialization: {ex.Message}");
                    if (ex.InnerException != null)
                    {
                        Debug.WriteLine($"Interior details: {ex.InnerException.Message}");
                    }

                    throw;
                }
            }
        }

        private async Task SeedDatabaseAsync()
        {
            var cities = new List<City>
            {
                new City { Name = "Milano", Country = "Italia", Latitude = 45.4642, Longitude = 9.1900 },
                new City { Name = "Parigi", Country = "Francia", Latitude = 48.8566, Longitude = 2.3522 },
                new City { Name = "Londra", Country = "Regno Unito", Latitude = 51.5074, Longitude = -0.1278 },
                new City { Name = "New York", Country = "USA", Latitude = 40.7128, Longitude = -74.0060 },
                new City { Name = "Tokyo", Country = "Giappone", Latitude = 35.6762, Longitude = 139.6503 }
            };

            try 
            {
                await _databaseConnection.InsertAllAsync(cities);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error when entering cities: {ex.Message}");
                throw;
            }
        }

        public async Task<int> AddCityAsync(City city)
        {
            await InitializeAsync();
            return await _databaseConnection.InsertAsync(city);
        }

        public async Task<List<City>> GetAllCityAsync()
        {
            await InitializeAsync();
            try
            {
                var cities = await _databaseConnection.Table<City>().ToListAsync();
                Debug.WriteLine($"Città recuperate: {cities.Count}");
                if (cities.Count == 0)
                {
                    Debug.WriteLine("No cities found in the database");
                }
                else
                {
                    foreach (var city in cities)
                    {
                        Debug.WriteLine($"Città: {city.Name}, Paese: {city.Country}");
                    }
                }

                return cities;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Errore nel recupero delle città: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Debug.WriteLine($"Dettagli interni: {ex.InnerException.Message}");
                }

                return new List<City>();
            }
        }

        public async Task<City> GetCityByIdAsync(long id)
        {
            await InitializeAsync();
            return await _databaseConnection.Table<City>().FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<int> UpdateCityAsync(City city)
        {
            await InitializeAsync();
            return await _databaseConnection.UpdateAsync(city);
        }

        public async Task<int> DeleteCityAsync(long id)
        {
            await InitializeAsync();
            return await _databaseConnection.DeleteAsync<City>(id);
        }
    }
}