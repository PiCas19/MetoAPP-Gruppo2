using MeteoApp;

namespace MeteoAPP;
/// <summary>
/// Rappresenta il contenitore principale della navigazione dell'app,
/// definendo le route per le pagine interne.
/// </summary>
public partial class AppShell : Shell
{
	/// <summary>
    /// Costruttore della classe <see cref="AppShell"/>.
    /// Inizializza i componenti e registra le rotte di navigazione.
    /// </summary>
	public AppShell()
	{
		InitializeComponent();
		Routing.RegisterRoute(nameof(MeteoItemPage), typeof(MeteoItemPage));
		Routing.RegisterRoute(nameof(AddItemPage), typeof(AddItemPage));
	}
}
