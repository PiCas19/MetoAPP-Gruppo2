namespace MeteoApp;
using MeteoApp.Models;
using MeteoApp.Services;
using System.Diagnostics;

public partial class MeteoListPage : Shell
{
    public Dictionary<string, Type> Routes { get; private set; } = new Dictionary<string, Type>();
    private WeatherDatabase _database;

    public MeteoListPage()
    {
        InitializeComponent();
        _database = App.Database;
        BindingContext = new MeteoListViewModel();
        LoadWeatherData();
        Routing.RegisterRoute("entrydetails", typeof(MeteoItemPage));
    }

    private async void LoadWeatherData()
    {
        try
        {
            var weatherDataList = await _database.GetWeatherDataAsync();
            Console.WriteLine($"Loaded {weatherDataList.Count} weather data items from database!");

            if (BindingContext is MeteoListViewModel viewModel)
            {
                viewModel.WeatherDataList.Clear();
                foreach (var weather in weatherDataList)
                {
                    if (weather != null)
                    {
                        Console.WriteLine($"Adding weather data for location: {weather.Location?.Name ?? "N/A"}");
                        viewModel.WeatherDataList.Add(weather);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading weather data: {ex.Message}");
            await DisplayAlert("Error", $"Failed to load weather data: {ex.Message}", "OK");
        }
    }

    private async void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection == null || e.CurrentSelection.Count == 0)
            return;

        if (e.CurrentSelection.FirstOrDefault() is WeatherData selectedWeather)
        {
            Console.WriteLine($"Navigating to entrydetails with: {selectedWeather.Location?.Name ?? "NULL"}");

            var navigationParameter = new Dictionary<string, object>
            {
                { "location", selectedWeather.Location.Name ?? "Sconosciuto" },
                { "temperature", selectedWeather.Temperature?.Current.ToString("F1", System.Globalization.CultureInfo.InvariantCulture) ?? "N/A" },
                { "minTemperature", selectedWeather.Temperature?.Min.ToString("F1", System.Globalization.CultureInfo.InvariantCulture) ?? "N/A" },
                { "maxTemperature", selectedWeather.Temperature?.Max.ToString("F1", System.Globalization.CultureInfo.InvariantCulture) ?? "N/A" },
                { "description", selectedWeather.Weather?.Description ?? "N/A" },
                { "icon", selectedWeather.Weather?.Icon ?? "" }
            };

            await Shell.Current.GoToAsync("entrydetails", navigationParameter);
        }
        ((CollectionView)sender).SelectedItem = null;
    }


    private async void OnItemAdded(object sender, EventArgs e)
    {
      await Navigation.PushAsync(new AddItemPage());
    }
}
