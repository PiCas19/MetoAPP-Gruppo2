﻿namespace MeteoApp;
using MeteoApp.Models;

public partial class MeteoListPage : Shell
{
    public Dictionary<string, Type> Routes { get; private set; } = new Dictionary<string, Type>();

    public MeteoListPage()
    {
        InitializeComponent();
        RegisterRoutes();

        BindingContext = new MeteoListViewModel();
    }

    private void RegisterRoutes()
    {
        Routes.Add("entrydetails", typeof(MeteoItemPage));

        foreach (var item in Routes)
            Routing.RegisterRoute(item.Key, item.Value);
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
