using SQLite;
using MeteoAPP.Models;
using System.Diagnostics;

namespace MeteoAPP.Services
{
    /// <summary>
    /// Gestisce la connessione e le operazioni sul database locale SQLite per la persistenza delle città.
    /// </summary>
    public class DatabaseService
    {
        private readonly SQLiteAsyncConnection _databaseConnection;
        private const string SeededKey = "DatabaseSeeded";
        private bool _isInitialized = false;

        /// <summary>
        /// Costruttore della classe <see cref="DatabaseService"/>.
        /// Inizializza la connessione al database SQLite.
        /// </summary>
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

        /// <summary>
        /// Inizializza il database creando la tabella e effettuando il seed iniziale se necessario.
        /// </summary>
        public async Task InitializeAsync()
        {
            if (!_isInitialized)
            {
                try
                {
                    await _databaseConnection.CreateTableAsync<City>();
                    await _databaseConnection.CreateTableAsync<WeatherHistory>();
                    var existingCities = await _databaseConnection.Table<City>().ToListAsync();

                    if (!existingCities.Any())
                    {
                        bool isSeeded = Preferences.Get(SeededKey, false);
                        if (!isSeeded)
                        {
                            await SeedDatabaseAsync();
                            Preferences.Set(SeededKey, true);
                        }
                    }

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
        /// <summary>
        /// Popola il database con un set di città iniziali.
        /// </summary>
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
        /// <summary>
        /// Aggiunge una nuova città al database.
        /// </summary>
        /// <param name="city">Oggetto <see cref="City"/> da inserire.</param>
        /// <returns>ID della riga inserita.</returns>
        public async Task<int> AddCityAsync(City city)
        {
            await InitializeAsync();
            return await _databaseConnection.InsertAsync(city);
        }

        /// <summary>
        /// Recupera tutte le città salvate nel database.
        /// </summary>
        /// <returns>Lista di oggetti <see cref="City"/>.</returns>
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
        /// <summary>
        /// Recupera una città specifica tramite il suo ID.
        /// </summary>
        /// <param name="id">ID univoco della città.</param>
        /// <returns>Oggetto <see cref="City"/> corrispondente o null se non trovato.</returns>
        public async Task<City> GetCityByIdAsync(long id)
        {
            await InitializeAsync();
            return await _databaseConnection.Table<City>().FirstOrDefaultAsync(c => c.Id == id);
        }
        /// <summary>
        /// Aggiorna una città esistente nel database.
        /// </summary>
        /// <param name="city">Oggetto <see cref="City"/> con i dati aggiornati.</param>
        /// <returns>Numero di righe interessate.</returns>
        public async Task<int> UpdateCityAsync(City city)
        {
            await InitializeAsync();
            return await _databaseConnection.UpdateAsync(city);
        }

        /// <summary>
        /// Elimina una città dal database utilizzando il suo ID.
        /// </summary>
        /// <param name="id">ID della città da eliminare.</param>
        /// <returns>Numero di righe eliminate.</returns>
        public async Task<int> DeleteCityAsync(long id)
        {
            await InitializeAsync();
            return await _databaseConnection.DeleteAsync<City>(id);
        }

        /// <summary>
        /// Aggiunge una nuova voce di cronologia meteo al database.
        /// </summary>
        /// <param name="history">Oggetto <see cref="WeatherHistory"/> contenente i dati meteo da salvare.</param>
        /// <returns>ID della riga inserita.</returns>
        public async Task<int> AddWeatherHistoryAsync(WeatherHistory history)
        {
            await InitializeAsync();
            return await _databaseConnection.InsertAsync(history);
        }

        public async Task ClearWeatherHistoryAsync()
        {
            await InitializeAsync();
            await _databaseConnection.ExecuteAsync("DELETE FROM WeatherHistory");
        }

        public async Task LogAllWeatherHistoryAsync()
        {
            var allEntries = await _databaseConnection.Table<WeatherHistory>().ToListAsync();
            foreach (var entry in allEntries)
            {
                Android.Util.Log.Debug("MeteoAPP", $"ID: {entry.Id}, City: {entry.CityName}, Date: {entry.Date.ToShortDateString()}, TempMin: {entry.TemperatureMin}, TempMax: {entry.TemperatureMax}, Desc: {entry.Description}");
            }
        }

        public async Task<List<WeatherHistory>> GetLast7DaysWeatherByCityAsync(string cityName)
        {
            await InitializeAsync();
            Android.Util.Log.Debug("MeteoAPP", $"GetLast7DaysWeatherByCityAsync - City: {cityName}");

            var result = await _databaseConnection.Table<WeatherHistory>()
                                                 .Where(wh => wh.CityName == cityName)
                                                 .ToListAsync();

            if (result == null)
                Android.Util.Log.Debug("MeteoAPP", $"Nessun dato trovato per la città {cityName}");
            else
                Android.Util.Log.Debug("MeteoAPP", $"Trovati {result.Count} record per la città {cityName}");

            return result;
        }

        /// <summary>
        /// Restituisce tutti i record di storico meteo.
        /// </summary>
        public async Task<List<WeatherHistory>> GetAllWeatherHistoryAsync()
        {
            await InitializeAsync();
            return await _databaseConnection.Table<WeatherHistory>().ToListAsync();
        }

        /// <summary>
        /// Sostituisce completamente le tabelle Cities e WeatherHistory con i dati forniti.
        /// </summary>
        public async Task ReplaceAllAsync(List<City> cities, List<WeatherHistory> history)
        {
            await InitializeAsync();

            // Esegue tutte le operazioni in una transazione
            await _databaseConnection.RunInTransactionAsync(conn =>
            {
                // Svuota le tabelle
                conn.DeleteAll<WeatherHistory>();
                conn.DeleteAll<City>();

                // Re-inserisce i dati (se presenti)
                if (cities != null && cities.Count > 0)
                    conn.InsertAll(cities);

                if (history != null && history.Count > 0)
                    conn.InsertAll(history);
            });
        }
    }
}