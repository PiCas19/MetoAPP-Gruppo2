using MeteoAPP.ViewModels;
using MeteoAPP.Models;
using MeteoAPP.Services;
using System.Diagnostics;
using MeteoApp;

namespace MeteoAPP
{
    public partial class ListMeteoPage : ContentPage
    {
        private readonly MeteoListViewModel _viewModel;
        private readonly OpenWeatherService _weatherService;
        private readonly GeoLocationService _locationService;

        public ListMeteoPage()
        {
            InitializeComponent();
            _locationService = new GeoLocationService();
            _weatherService = new OpenWeatherService();
            _ = _weatherService.InitializeAsync();
            _viewModel = new MeteoListViewModel(_locationService, _weatherService);
            BindingContext = _viewModel;
            Loaded += async (s, e) => await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            try
            {
                await _viewModel.LoadCitiesAsync();                
                await _viewModel.LoadCurrentLocationAsync();
                
                if (_viewModel.FilteredCities.Count == 0 && _viewModel.Cities.Count == 0)
                {
                    await DisplayAlert("Attention", "No cities available", "OK");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Page: Error loading cities: {ex.Message}");
                await DisplayAlert("Error", "Unable to upload cities", "OK");
            }
        }

        private async void OnAddCityClicked(object sender, EventArgs e)
        {
            try
            {
                var addItemPage = new AddItemPage(_viewModel, _locationService);
                await Navigation.PushAsync(addItemPage);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error while browsing: {ex.Message}", "OK");
            }
        }

        private async void OnCitySelected(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            if (e.Item is City selectedCity)
            {
                try
                {
                    var weather = await _weatherService.GetWeatherByCoordinatesAsync(selectedCity.Latitude, selectedCity.Longitude);
                    if (weather == null)
                    {
                        await DisplayAlert("Error", "Unable to upload weather data", "OK");
                        return;
                    }

                    var navigationParameter = new Dictionary<string, object>
                    {
                        { "CityName", selectedCity.Name ?? "Sconosciuto" },
                        { "Temperature", weather.Temperature.ToString("F1", System.Globalization.CultureInfo.InvariantCulture) },
                        { "TemperatureMin", weather.TemperatureMin.ToString("F1", System.Globalization.CultureInfo.InvariantCulture) },
                        { "TemperatureMax", weather.TemperatureMax.ToString("F1", System.Globalization.CultureInfo.InvariantCulture) },
                        { "Description", weather.Description ?? "N/A" },
                        { "Icon", weather.IconCode ?? "01d" }
                    };

                    await Shell.Current.GoToAsync("MeteoItemPage", navigationParameter);
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"Error during navigation: {ex.Message}", "OK");
                }
            }
        }

        private async void OnDeleteItemInvoked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is City city)
            {
                bool confirm = await DisplayAlert("Confirm", $"Do you want to eliminate {city.Name}?", "Yes", "No");
                if (confirm)
                {
                    try
                    {
                        await _viewModel.RemoveCityAsync(city);
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("Error", $"Error during elimination: {ex.Message}", "OK");
                    }
                }
            }
        }

         private async void OnSettingsButtonClicked(object sender, EventArgs e)
         {
            if (sender is Button button && button.CommandParameter is City city)
            {
                try
                {
                    var settingsViewModel = new NotificationSettingsViewModel(city);
                    var settingsPage = new NotificationSettingsPage(settingsViewModel);
                    await Navigation.PushModalAsync(settingsPage);
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"Unable to open settings: {ex.Message}", "OK");
                }
            }
         }

        private async void OnCurrentLocationTapped(object sender, EventArgs e)
        {
            try
            {
                LoadingIndicator.IsVisible = true;
                LoadingIndicator.IsRunning = true;

                var locationResult = await _locationService.GetCurrentLocationAsync();

                if (locationResult.Success)
                {
                    var weather = await _weatherService.GetWeatherByCoordinatesAsync(
                        locationResult.Latitude, 
                        locationResult.Longitude
                    );

                    if (weather == null)
                    {
                        await DisplayAlert("Error", "Unable to upload weather data", "OK");
                        return;
                    }

                    var navigationParameter = new Dictionary<string, object>
                    {
                        { "CityName", _viewModel.CurrentCityName },
                        { "Temperature", weather.Temperature.ToString("F1", System.Globalization.CultureInfo.InvariantCulture) },
                        { "TemperatureMin", weather.TemperatureMin.ToString("F1", System.Globalization.CultureInfo.InvariantCulture) },
                        { "TemperatureMax", weather.TemperatureMax.ToString("F1", System.Globalization.CultureInfo.InvariantCulture) },
                        { "Description", weather.Description ?? "N/A" },
                        { "Icon", weather.IconCode ?? "01d" }
                    };

                    await Shell.Current.GoToAsync("MeteoItemPage", navigationParameter);
                }
                else
                {
                    await DisplayAlert("Errore", locationResult.ErrorMessage, "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error during position recovery: {ex.Message}", "OK");
            }
            finally
            {
                LoadingIndicator.IsVisible = false;
                LoadingIndicator.IsRunning = false;
            }
        }
    }
}