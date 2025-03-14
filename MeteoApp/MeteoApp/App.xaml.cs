namespace MeteoApp;
using MeteoApp.Services;

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
                Console.WriteLine(dbPath);
                _database = new WeatherDatabase(dbPath);
            }
            return _database;
        }
    }
    
    public App()
    {
        InitializeComponent();
		_ = InitializeDatabaseAsync();
        MainPage = new MeteoListPage();
    }

	private async Task InitializeDatabaseAsync()
	{
		var database = Database;
		await DatabaseSeeder.SeedDatabaseAsync(database);
	}
}
