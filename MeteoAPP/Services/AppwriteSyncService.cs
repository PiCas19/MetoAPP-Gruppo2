using Appwrite;
using Appwrite.Models;
using Appwrite.Services;
using Newtonsoft.Json;
using MeteoAPP.Models;
using System.Diagnostics;
using IOFile = System.IO.File;

namespace MeteoAPP.Services
{
    /// <summary>
    /// Gestisce la sincronizzazione dei dati tra il database locale e Appwrite (cloud).
    /// </summary>
    public class AppwriteSyncService
    {
        private Client? _client;
        private Databases? _databases;
        private readonly DatabaseService _databaseService;
        private string _databaseId = "";
        private string _collectionId = "";

        /// <summary>
        /// Costruttore che accetta il servizio locale ma non inizializza Appwrite subito.
        /// </summary>
        public AppwriteSyncService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        /// <summary>
        /// Inizializza Appwrite e carica la configurazione dal file config.json.
        /// Va chiamato manualmente dopo la costruzione.
        /// </summary>
        public async Task InitializeAsync()
        {
            await ConfigService.Instance.InitializeAsync();
            var config = ConfigService.Instance.Config;

            if (config == null)
                throw new InvalidOperationException("⚠️ Config non trovata");

            _client = new Client()
                .SetEndpoint("https://cloud.appwrite.io/v1")
                .SetProject(config.AppwriteProjectId)
                .SetKey(config.AppwriteApiKey);

            _databases = new Databases(_client);
            _databaseId = config.AppwriteDatabaseId;
            _collectionId = config.AppwriteCollectionId;
        }
        /// <summary>
        /// Elimina una città dal database Appwrite in base all’ID.
        /// </summary>
        public async Task DeleteCityFromAppwriteAsync(long cityId)
        {
            try
            {
                string deviceName = DeviceInfo.Name;

                var response = await _databases!.ListDocuments(
                    _databaseId, _collectionId,
                    new List<string>
                    {
                        Query.Equal("device", deviceName),
                        Query.Equal("idCity", cityId)
                    });

                foreach (var doc in response.Documents)
                {
                    try
                    {
                        await _databases.DeleteDocument(_databaseId, _collectionId, doc.Id);
                        Debug.WriteLine($"🗑️ Documento eliminato per cityId: {cityId}");
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"❌ Errore eliminazione: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Errore DeleteCityFromAppwriteAsync: {ex.Message}");
            }
        }


        /// <summary>
        /// Invia una singola città al database Appwrite.
        /// </summary>
        public async Task PushCityToAppwriteAsync(City city)
        {
            try
            {
                string deviceName = DeviceInfo.Name;

                var data = new Dictionary<string, object>
                {
                    { "idCity", city.Id },
                    { "name", city.Name },
                    { "country", city.Country },
                    { "latitude", city.Latitude },
                    { "longitude", city.Longitude },
                    { "device", deviceName }
                };

                await _databases!.CreateDocument(_databaseId, _collectionId, ID.Unique(), data);
                Debug.WriteLine($"✅ Città '{city.Name}' inviata ad Appwrite.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Errore PushCityToAppwriteAsync: {ex.Message}");
            }
        }

        /// <summary>
        /// Invia tutte le città locali ad Appwrite.
        /// </summary>
        public async Task SyncCitiesToAppwriteAsync()
        {
            try
            {
                var cities = await _databaseService.GetAllCityAsync();
                foreach (var city in cities)
                {
                    await PushCityToAppwriteAsync(city);
                }

                Debug.WriteLine("✅ Tutte le città sincronizzate.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Errore SyncCitiesToAppwriteAsync: {ex.Message}");
            }
        }

        /// <summary>
        /// Scarica tutte le città da Appwrite associate a questo dispositivo e le salva localmente.
        /// </summary>
        public async Task PullCitiesFromAppwriteAsync()
        {
            try
            {
                string deviceName = DeviceInfo.Name;
                var response = await _databases!.ListDocuments(
                    _databaseId,
                    _collectionId,
                    new List<string> { Query.Equal("device", deviceName) });

                var existingCities = await _databaseService.GetAllCityAsync();
                var existingIds = existingCities.Select(c => c.Id).ToHashSet();

                foreach (var doc in response.Documents)
                {
                    if (doc.Data is not IDictionary<string, object> data)
                        continue;

                    long idCity = data.ContainsKey("idCity") ? Convert.ToInt64(data["idCity"]) : 0;
                    if (existingIds.Contains(idCity)) continue;

                    var city = new City
                    {
                        Id = idCity,
                        Name = data["name"]?.ToString() ?? "",
                        Country = data["country"]?.ToString() ?? "",
                        Latitude = Convert.ToDouble(data["latitude"]),
                        Longitude = Convert.ToDouble(data["longitude"])
                    };

                    await _databaseService.AddCityAsync(city);
                    Debug.WriteLine($"✅ Città '{city.Name}' importata da Appwrite.");
                }

                Debug.WriteLine("✅ Pull completato.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Errore PullCitiesFromAppwriteAsync: {ex.Message}");
            }
        }
    }
}
