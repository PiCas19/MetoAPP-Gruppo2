namespace MeteoApp;
using MeteoApp.Services;
using System.Diagnostics;

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
        Task.Run(async () =>
        {
            try
            {
                await InitializeDatabaseAsync();
                Debug.WriteLine("Database inizializzato correttamente.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Errore nel database: {ex.Message}");
            }
        }).Wait(); 
    }

    protected override Window CreateWindow(IActivationState activationState)
    {
        return new Window(new MeteoListPage()); 
    }
}
