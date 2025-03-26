using MeteoAPP.Services;

namespace MeteoAPP;

public partial class App : Application
{
	public static DatabaseService? DatabaseService { get; private set; }

    public App()
    {
        InitializeComponent();
        DatabaseService = new DatabaseService();
    }

    protected override async void OnStart()
    {
        if (DatabaseService != null)
        {
            try 
            {
                await DatabaseService.InitializeAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore inizializzazione App: {ex.Message}");
            }
        }
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new AppShell());
    }
}