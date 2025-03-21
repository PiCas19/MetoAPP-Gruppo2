namespace MeteoApp;
using MeteoApp.Services;
using System.Diagnostics;
using Android.Webkit;

public partial class App : Application
{
    private static WeatherDatabase _database;

    public static WeatherDatabase Database
    {
        get
        {
            if (_database == null)
            {
                string dbPath = Path.Combine(FileSystem.AppDataDirectory, "WeatherDB.db3");
                Debug.WriteLine($"Database Path: {dbPath}");
                _database = new WeatherDatabase(dbPath);
            }
            return _database;
        }
    }

    private async Task InitializeDatabaseAsync()
    {
        await Database.InitializeAsync();
        await DatabaseSeeder.SeedDatabaseAsync(Database);
    }

    public App()
    {
        InitializeComponent();    
    }

    protected override void OnStart()
    {
        base.OnStart();
        Task.Run(async () =>
        {
            try
            {
                await InitializeDatabaseAsync();
                Debug.WriteLine("Database ricaricato all'avvio dell'app.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Errore durante il caricamento del database all'avvio: {ex.Message}");
            }
        }).Wait();
    }

    protected override Window CreateWindow(IActivationState activationState)
    {
        return new Window(new MeteoListPage()); 
    }
}
