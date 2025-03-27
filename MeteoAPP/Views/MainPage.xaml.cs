namespace MeteoAPP;

public partial class MainPage : ContentPage
{
	int count = 0;

	public MainPage()
	{
		InitializeComponent();
	}

	private async void OnStartClicked(object sender, EventArgs e)
	{
		await Navigation.PushAsync(new ListMeteoPage()); 
	}
}

