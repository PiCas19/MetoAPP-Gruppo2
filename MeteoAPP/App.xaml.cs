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
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore inizializzazione App: {ex.Message}");
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