
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using MeteoAPP.Models;
using Microsoft.Maui.Storage;
using SQLite;

namespace MeteoAPP.Services
{
    /// <summary>
    /// Gestisce esportazione e importazione di tutte le entità dell'app da/verso file JSON.
    /// Il file viene salvato/letto nella cartella locale dell'app (FileSystem.AppDataDirectory).
    /// </summary>
    public class ExportImportService
    {
        private readonly DatabaseService _db;
        private readonly JsonSerializerOptions _jsonOptions;

        private const string DefaultFileName = "meteoapp-backup.json";
        private const string ExportVersion = "1.0";

        public ExportImportService(DatabaseService db)
        {
            _db = db;
            _jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
        }

        private record ExportRoot(
            string Version,
            DateTime ExportedAtUtc,
            List<City> Cities,
            List<WeatherHistory> History,
            Dictionary<string, object> Preferences
        );

        /// <summary>
        /// Esegue l'esportazione completa su JSON. Ritorna il percorso del file creato.
        /// </summary>
        public async Task<string> ExportAsync(string? fileName = null, CancellationToken ct = default)
        {
            await _db.InitializeAsync();

            var cities = await _db.GetAllCityAsync();
            var history = await _db.GetAllWeatherHistoryAsync();

            // Raccogliamo preferenze locali note (espandibile in futuro)
            var prefs = new Dictionary<string, object>
            {
                { "DatabaseSeeded", Preferences.Get("DatabaseSeeded", false) }
            };

            var root = new ExportRoot(
                Version: ExportVersion,
                ExportedAtUtc: DateTime.UtcNow,
                Cities: cities,
                History: history,
                Preferences: prefs
            );

            var json = JsonSerializer.Serialize(root, _jsonOptions);
            var folder = FileSystem.AppDataDirectory;
            var path = Path.Combine(folder, string.IsNullOrWhiteSpace(fileName) ? DefaultFileName : fileName);
            await File.WriteAllTextAsync(path, json, Encoding.UTF8, ct);
            return path;
        }

        /// <summary>
        /// Importa uno stato app da un file JSON nella cartella locale dell'app.
        /// Esegue validazione di schema e dati prima di scrivere nel database.
        /// </summary>
        public async Task ImportAsync(string? fileName = null, CancellationToken ct = default)
        {
            var folder = FileSystem.AppDataDirectory;
            var path = Path.Combine(folder, string.IsNullOrWhiteSpace(fileName) ? DefaultFileName : fileName);

            if (!File.Exists(path))
                throw new FileNotFoundException($"File non trovato nella cartella locale: {Path.GetFileName(path)}", path);

            var json = await File.ReadAllTextAsync(path, Encoding.UTF8, ct);

            // Parsing in document model dinamico per validazione preventiva
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            // Validazione minima dello schema
            Require(root, "Version", JsonValueKind.String);
            Require(root, "ExportedAtUtc", JsonValueKind.String); // ISO datetime
            Require(root, "Cities", JsonValueKind.Array);
            Require(root, "History", JsonValueKind.Array);
            if (root.TryGetProperty("Preferences", out var prefsElem) && prefsElem.ValueKind != JsonValueKind.Object)
                throw new InvalidDataException("Campo 'Preferences' deve essere un oggetto.");

            // Validazione contenuto Cities
            foreach (var c in root.GetProperty("Cities").EnumerateArray())
            {
                Require(c, "Name", JsonValueKind.String);
                Require(c, "Country", JsonValueKind.String);
                Require(c, "Latitude", JsonValueKind.Number);
                Require(c, "Longitude", JsonValueKind.Number);

                var lat = c.GetProperty("Latitude").GetDouble();
                var lon = c.GetProperty("Longitude").GetDouble();
                if (lat < -90 || lat > 90) throw new InvalidDataException($"Latitudine fuori range: {lat}");
                if (lon < -180 || lon > 180) throw new InvalidDataException($"Longitudine fuori range: {lon}");
            }

            // Validazione contenuto History
            foreach (var h in root.GetProperty("History").EnumerateArray())
            {
                Require(h, "CityName", JsonValueKind.String);
                Require(h, "Date", JsonValueKind.String);
                // Temperature min/max opzionali ma se presenti devono essere numeriche
                if (h.TryGetProperty("TempMin", out var _tmpMin) && _tmpMin.ValueKind != JsonValueKind.Number)
                    throw new InvalidDataException("TempMin deve essere numerico.");
                if (h.TryGetProperty("TempMax", out var _tmpMax) && _tmpMax.ValueKind != JsonValueKind.Number)
                    throw new InvalidDataException("TempMax deve essere numerico.");
            }

            // Se la validazione passa, deserializziamo nelle classi forti
            var model = JsonSerializer.Deserialize<ImportRoot>(json, _jsonOptions)
                        ?? throw new InvalidDataException("JSON non valido per la deserializzazione.");

            // Scriviamo nel DB in una transazione
            await _db.InitializeAsync();
            await _db.ReplaceAllAsync(model.Cities, model.History);

            // Ripristino preferenze note
            if (model.Preferences.TryGetValue("DatabaseSeeded", out var seededObj) && seededObj is bool seeded)
            {
                Preferences.Set("DatabaseSeeded", seeded);
            }
        }

        private static void Require(JsonElement el, string prop, JsonValueKind kind)
        {
            if (!el.TryGetProperty(prop, out var v) || v.ValueKind != kind)
                throw new InvalidDataException($"Campo obbligatorio '{prop}' mancante o di tipo errato.");
        }

        private class ImportRoot
        {
            public string Version { get; set; } = "";
            public DateTime ExportedAtUtc { get; set; }
            public List<City> Cities { get; set; } = new();
            public List<WeatherHistory> History { get; set; } = new();
            public Dictionary<string, object> Preferences { get; set; } = new();
        }
    }
}
