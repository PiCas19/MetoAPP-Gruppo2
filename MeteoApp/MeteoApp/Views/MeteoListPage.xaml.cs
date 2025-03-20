namespace MeteoApp;
using MeteoApp.Models;
using MeteoApp.Services;

public partial class MeteoListPage : Shell
{
    private WeatherDatabase _database;
    public Dictionary<string, Type> Routes { get; private set; } = new Dictionary<string, Type>();
    
    public MeteoListPage()
    {
        _database = App.Database;
        
        BindingContext = new MeteoListViewModel();
        
        RegisterRoutes();
        LoadWeatherData();
    }
    
    private void RegisterRoutes()
    {
        Routes.Add("entrydetails", typeof(MeteoItemPage));
        foreach (var item in Routes)
            Routing.RegisterRoute(item.Key, item.Value);
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
                        Console.WriteLine($"Adding weather data for location: {weather.LocationJson}");
                        viewModel.WeatherDataList.Add(weather);
                    }
                }
                Console.WriteLine($"Added {viewModel.WeatherDataList.Count} items to view model");
            }
            else
            {
                Console.WriteLine("BindingContext is not a MeteoListViewModel");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading weather data: {ex.Message}");
            await DisplayAlert("Error", $"Failed to load weather data: {ex.Message}", "OK");
        }
    }
    
    private void OnListItemSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.Count > 0)
        {
            WeatherData selectedWeather = e.CurrentSelection[0] as WeatherData;
            if (selectedWeather != null)
            {
                var navigationParameter = new Dictionary<string, object>
                {
                    { "Location", selectedWeather.Location },
                    { "Weather", selectedWeather.Weather },
                    { "Temperature", selectedWeather.Temperature },
                    { "Wind", selectedWeather.Wind },
                    { "Cloud", selectedWeather.Cloud },
                    { "Rain", selectedWeather.Rain },
                    { "Snow", selectedWeather.Snow },
                    { "Sunrise", selectedWeather.Sunrise },
                    { "Sunset", selectedWeather.Sunset },
                    { "Timezone", selectedWeather.Timezone }
                };
                Shell.Current.GoToAsync("entrydetails", navigationParameter);
            }
            ((CollectionView)sender).SelectedItem = null;
        }
    }
    
    private void OnItemAdded(object sender, EventArgs e)
    {
        _ = ShowPrompt();
    }
    
    private async Task ShowPrompt()
    {
        await DisplayAlert("Add City", "To Be Implemented", "OK");
    }
}
