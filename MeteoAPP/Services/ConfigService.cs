using MeteoAPP.Models;
using Newtonsoft.Json;
using System.Diagnostics;

namespace MeteoAPP.Services
{
    /// <summary>
    /// Caricatore singleton per le impostazioni da config.json
    /// </summary>
    public class ConfigService
    {
        private static ConfigService? _instance;
        private static readonly object _lock = new();

        public Config? Config { get; private set; }

        private ConfigService() { }

        public static ConfigService Instance
        {
            get
            {
                lock (_lock)
                {
                    return _instance ??= new ConfigService();
                }
            }
        }

        /// <summary>
        /// Carica la configurazione da disco o embedded (solo una volta)
        /// </summary>
        public async Task InitializeAsync()
        {
            if (Config != null) return;

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
                        Config = JsonConvert.DeserializeObject<Config>(jsonContent);
                        if (Config != null) return;
                    }
                }

                var configFilePath = Path.Combine(FileSystem.AppDataDirectory, "config.json");

                if (File.Exists(configFilePath))
                {
                    var fileContent = await File.ReadAllTextAsync(configFilePath);
                    Config = JsonConvert.DeserializeObject<Config>(fileContent);
                }
                else
                {
                    Debug.WriteLine("⚠️ config.json non trovato su disco.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("❌ Errore caricamento config: " + ex.Message);
            }
        }

        public string? GetBaseApiUrl() => Config?.NotificationSettingsBaseUrl?.TrimEnd('/');
        public string? GetOpenWeatherApiKey() => Config?.OpenWeatherApiKey;
        public string? GetWeatherApiKey() => Config?.WeatherApiKey;
        public string? GetAppwriteProjectId() => Config?.AppwriteProjectId;
        public string? GetAppwriteApiKey() => Config?.AppwriteApiKey;
        public string? GetAppwriteDatabaseId() => Config?.AppwriteDatabaseId;
        public string? GetAppwriteCollectionId() => Config?.AppwriteCollectionId;
    }
}
