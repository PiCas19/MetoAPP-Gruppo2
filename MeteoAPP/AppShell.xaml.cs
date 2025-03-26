using MeteoApp;

namespace MeteoAPP;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		Routing.RegisterRoute(nameof(MeteoItemPage), typeof(MeteoItemPage));
		Routing.RegisterRoute(nameof(AddItemPage), typeof(AddItemPage));
	}
}
