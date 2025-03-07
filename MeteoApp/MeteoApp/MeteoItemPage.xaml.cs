namespace MeteoApp;
using MeteoApp.Models;

[QueryProperty(nameof(Location), "Location")]
public partial class MeteoItemPage : ContentPage
{
    private WeatherLocation location;

    public WeatherLocation Location
    {
        get => location;
        set
        {
            location = value;
            OnPropertyChanged();
        }
    }

    public MeteoItemPage()
    {
        InitializeComponent();
        BindingContext = this;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
    }
}
