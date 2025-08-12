using MeteoAPP.Services;

namespace MeteoAPP;

/// <summary>
/// Classe principale dell'applicazione MeteoAPP.
/// Responsabile dell'inizializzazione dei servizi di database locale e sincronizzazione cloud.
/// </summary>
public partial class App : Application
{
    /// <summary>
    /// Istanza statica del servizio di database locale SQLite.
    /// </summary>
	public static DatabaseService? DatabaseService { get; private set; }

    /// <summary>
    /// Istanza statica del servizio di sincronizzazione con Appwrite.
    /// </summary>
    public static AppwriteSyncService? AppwriteSyncService { get; private set; }

    /// <summary>
    /// Costruttore dell'applicazione.
    /// Inizializza i componenti e i servizi globali.
    /// </summary>
    public App()
    {
        InitializeComponent();
        DatabaseService = new DatabaseService();
        AppwriteSyncService = new AppwriteSyncService(DatabaseService);
    }

    /// <summary>
    /// Metodo eseguito all'avvio dell'applicazione.
    /// Inizializza il database locale e sincronizza i dati da Appwrite.
    /// </summary>
    protected override async void OnStart()
    {

        if (DatabaseService != null)
        {
            try
            {
                await DatabaseService.InitializeAsync();
                await AppwriteSyncService!.InitializeAsync();
                await AppwriteSyncService!.PullCitiesFromAppwriteAsync();
                var cities = await App.DatabaseService.GetAllCityAsync();
                var weatherService = new OpenWeatherService();

                foreach (var city in cities)
                {
                    var existingHistory = await App.DatabaseService.GetLast7DaysWeatherByCityAsync(city.Name);

                    bool hasToday = existingHistory.Any(h => h.Date == DateTime.UtcNow.Date);

                    if (!hasToday)
                    {
                        var weather = await weatherService.GetWeatherByCoordinatesAsync(city.Latitude, city.Longitude);
                        var history = new Models.WeatherHistory
                        {
                            CityName = city.Name,
                            Date = DateTime.UtcNow.Date,
                            TemperatureMin = weather?.TemperatureMin ?? 0,
                            TemperatureMax = weather?.TemperatureMax ?? 0,
                            Description = weather?.Description ?? "N/A",
                        };
                        await App.DatabaseService.AddWeatherHistoryAsync(history);
                        await DatabaseService.LogAllWeatherHistoryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore inizializzazione AppShell: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Crea la finestra principale dell'app associata al contenitore di navigazione <see cref="AppShell"/>.
    /// </summary>
    /// <param name="activationState">Stato di attivazione dell'applicazione.</param>
    /// <returns>Una nuova finestra contenente <see cref="AppShell"/>.</returns>
    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new AppShell());
    }
}