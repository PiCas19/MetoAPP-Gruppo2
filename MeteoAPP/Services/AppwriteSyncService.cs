using Appwrite;
using Appwrite.Models;
using Appwrite.Services;
using Newtonsoft.Json;
using MeteoAPP.Models;
using System.Diagnostics;
using System.Text.Json;

namespace MeteoAPP.Services {
    public class AppwriteSyncService
    {
        private readonly Client _client;
        private readonly Databases _databases;
        private readonly DatabaseService _databaseService;
        private readonly string _databaseId;
        private readonly string _collectionId;


        public AppwriteSyncService(DatabaseService databaseService)
        {
            _databaseService = databaseService;

            var config = LoadConfig();

             _client = new Client()
            .SetEndpoint("https://cloud.appwrite.io/v1")
            .SetProject(config.AppwriteProjectId)
            .SetKey(config.AppwriteApiKey);

            _databases = new Databases(_client);
            _databaseId = config.AppwriteDatabaseId;
            _collectionId = config.AppwriteCollectionId;
        }

        private Config LoadConfig()
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
                    var jsonContent = reader.ReadToEnd();
                    var config = JsonConvert.DeserializeObject<Config>(jsonContent);
                    return config ?? new Config();
                }
            }

            var configFilePath = Path.Combine(FileSystem.AppDataDirectory, "config.json");
            if (!File.Exists(configFilePath))
            {
                File.WriteAllText(configFilePath, JsonConvert.SerializeObject(new Config()));
            }

            var fileContent = File.ReadAllText(configFilePath);
            return JsonConvert.DeserializeObject<Config>(fileContent) ?? new Config();
        }
        public async Task DeleteCityFromAppwriteAsync(long cityId)
        {
            try
            {
                string deviceName = DeviceInfo.Name;
                var response = await _databases.ListDocuments(
                    databaseId: DATABASE_ID,
                    collectionId: COLLECTION_ID,
                    queries: new List<string> {
                        Query.Equal("device", deviceName),
                        Query.Equal("idCity", cityId)
                    }
                );

                foreach (var doc in response.Documents)
                {
                    try
                    {
                        await _databases.DeleteDocument(
                            databaseId: DATABASE_ID,
                            collectionId: COLLECTION_ID,
                            documentId: doc.Id
                        );
                        Debug.WriteLine($"🗑️ Documento eliminato per cityId: {cityId}");
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"❌ Errore durante l'eliminazione del documento: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Errore generale nel delete da Appwrite: {ex.Message}");
            }
        }

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

                await _databases.CreateDocument(
                    databaseId: DATABASE_ID,
                    collectionId: COLLECTION_ID,
                    documentId: ID.Unique(),
                    data: data
                );

                Debug.WriteLine($"✅ Città '{city.Name}' sincronizzata con Appwrite.");
            }
            catch (AppwriteException ex)
            {
                Debug.WriteLine($"❌ Errore Appwrite: {ex.Message}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Errore generico durante il push: {ex.Message}");
            }
        }


        public async Task SyncCitiesToAppwriteAsync()
        {
            try
            {
                var cities = await _databaseService.GetAllCityAsync();
                string deviceName = DeviceInfo.Name;

                foreach (var city in cities)
                {
                    var data = new Dictionary<string, object>
                    {
                        { "idCity", city.Id },
                        { "name", city.Name },
                        { "country", city.Country },
                        { "latitude", city.Latitude },
                        { "longitude", city.Longitude },
                        { "device", deviceName }
                    };

                    try
                    {
                        await _databases.CreateDocument(
                            databaseId: DATABASE_ID,
                            collectionId: COLLECTION_ID,
                            documentId: ID.Unique(),
                            data: data
                        );
                    }
                    catch (AppwriteException ex)
                    {
                        Debug.WriteLine($"Errore salvataggio città {city.Name}: {ex.Message}");
                    }
                }

                Debug.WriteLine("✅ Sincronizzazione completata.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Errore nella sincronizzazione con Appwrite: {ex.Message}");
            }
        }

        public async Task PullCitiesFromAppwriteAsync()
        {
            try
            {
                string deviceName = DeviceInfo.Name;
                Debug.WriteLine($"🔄 Sync: Pulling cities for device: {deviceName}");

                var response = await _databases.ListDocuments(
                    databaseId: DATABASE_ID,
                    collectionId: COLLECTION_ID,
                    queries: new List<string> { Query.Equal("device", deviceName) }
                );

                Debug.WriteLine($"📡 Found {response.Documents.Count} documents on Appwrite");

                // Prendi tutti i cityId già presenti nel database SQLite
                var existingCities = await _databaseService.GetAllCityAsync();
                var existingCityIds = existingCities.Select(c => c.Id).ToHashSet();

                foreach (var doc in response.Documents)
                {
                    try
                    {
                        if (doc.Data is not IDictionary<string, object> data)
                        {
                            Debug.WriteLine("❌ Documento malformato o vuoto");
                            continue;
                        }

                        long cityId = data.ContainsKey("idCity") ? Convert.ToInt64(data["idCity"]) : 0;
                        if (existingCityIds.Contains(cityId))
                        {
                            Debug.WriteLine($"⏭️ Città già presente localmente (ID: {cityId}), skip.");
                            continue;
                        }

                        var city = new City
                        {
                            Id = cityId,
                            Name = data.ContainsKey("name") ? data["name"]?.ToString() ?? "" : "",
                            Country = data.ContainsKey("country") ? data["country"]?.ToString() ?? "" : "",
                            Latitude = data.ContainsKey("latitude") ? Convert.ToDouble(data["latitude"]) : 0.0,
                            Longitude = data.ContainsKey("longitude") ? Convert.ToDouble(data["longitude"]) : 0.0
                        };

                        await _databaseService.AddCityAsync(city);
                        Debug.WriteLine($"✅ Inserita città da cloud: {city.Name}");
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"⚠️ Errore parsing o inserimento DB locale: {ex.Message}");
                    }
                }

                Debug.WriteLine("✅ Pull completato");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Errore durante il recupero da Appwrite: {ex.Message}");
            }
        }

    }
}
