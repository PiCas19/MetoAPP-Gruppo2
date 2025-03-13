namespace MeteoApp;
using MeteoApp.Models;

[QueryProperty(nameof(Location), "Location")]
[QueryProperty(nameof(Weather), "Weather")]
[QueryProperty(nameof(Temperature), "Temperature")]
public partial class MeteoItemPage : ContentPage
{
    private WeatherLocation location;
    private WeatherInfo weather;
    private TemperatureInfo temperature;
    
    public WeatherLocation Location
    {
        get => location;
        set
        {
            location = value;
            OnPropertyChanged();
        }
    }

    public WeatherInfo Weather
    {
        get => weather;
        set
        {
            weather = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(WeatherIconUrl)); // Aggiorna anche l'icona
        }
    }

    public TemperatureInfo Temperature
    {
        get => temperature;
        set
        {
            temperature = value;
            OnPropertyChanged();
        }
    }

    public string WeatherIconUrl => weather != null && !string.IsNullOrEmpty(weather.Icon) 
        ? $"https://openweathermap.org/img/wn/{weather.Icon}.png"
        : string.Empty;

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
