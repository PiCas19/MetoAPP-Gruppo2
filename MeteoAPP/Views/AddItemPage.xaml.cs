using System.Text.Json;
using MeteoAPP.Models;
using MeteoAPP.ViewModels;
using MeteoAPP.Services;

namespace MeteoApp;

[QueryProperty("ViewModel", "ViewModel")]
[QueryProperty ("GeoLocationService", "GeoLocationService")]
public partial class AddItemPage : ContentPage
{
    private readonly MeteoListViewModel _meteoListViewModel;
    private readonly AddItemViewModel _addItemViewModel;
    private readonly GeoLocationService _locationService;

    public AddItemPage(MeteoListViewModel viewModel, GeoLocationService geoLocationService )
    {
        InitializeComponent();
        _meteoListViewModel = viewModel;
        _addItemViewModel = new AddItemViewModel();
        _locationService = geoLocationService;
        BindingContext = _addItemViewModel;

        #if ANDROID
                MapWebView.Source = "file:///android_asset/map.html";
        #else
                MapWebView.Source = "map.html";
        #endif

        MapWebView.Navigating += WebView_Navigating!;
        MapWebView.Navigated += OnWebViewNavigated!;

        _ = RequestLocationPermissionAndGetLocation();
    }

    private async Task RequestLocationPermissionAndGetLocation()
    {
        try
        {
            LoadingIndicator.IsVisible = true;
            LoadingIndicator.IsRunning = true;

            var locationResult = await _locationService.GetCurrentLocationAsync();
            
            if (locationResult.Success)
            {
                _addItemViewModel.Latitude = locationResult.Latitude;
                _addItemViewModel.Longitude = locationResult.Longitude;

                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    string city = await _addItemViewModel.GetLocationNameAsync(locationResult.Latitude, locationResult.Longitude);
                    CityLabel.Text = $"{_addItemViewModel.CityName}, {_addItemViewModel.CountryName}";
                    CitySearchEntry.Text = _addItemViewModel.CityName;
                    string js = $"updateLocation({locationResult.Latitude}, {locationResult.Longitude});";
                    await MapWebView.EvaluateJavaScriptAsync(js);
                });
            }
            else
            {
                await DisplayAlert("Errore", locationResult.ErrorMessage, "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Errore", $"Si è verificato un problema: {ex.Message}", "OK");
        }
        finally
        {
            LoadingIndicator.IsVisible = false;
            LoadingIndicator.IsRunning = false;
        }
    }
    private async void WebView_Navigating(object sender, WebNavigatingEventArgs e)
    {
        if (e.Url.StartsWith("js://update-location/"))
        {
            e.Cancel = true;

            string data = Uri.UnescapeDataString(e.Url.Replace("js://update-location/", ""));
            try
            {
                var coordinates = JsonSerializer.Deserialize<Dictionary<string, double>>(data);
                if (coordinates?.ContainsKey("lat") == true && coordinates.ContainsKey("lng"))
                {
                    _addItemViewModel.Latitude = coordinates["lat"];
                    _addItemViewModel.Longitude = coordinates["lng"];

                    string city = await _addItemViewModel.GetLocationNameAsync(_addItemViewModel.Latitude, _addItemViewModel.Longitude);

                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        CityLabel.Text = $"{_addItemViewModel.CityName}, {_addItemViewModel.CountryName}";
                        CitySearchEntry.Text = _addItemViewModel.CityName;
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore durante la deserializzazione: {ex.Message}");
            }
        }
    }

    private async void OnSearchButtonClicked(object sender, EventArgs e)
    {
        string searchText = CitySearchEntry.Text;

        if (string.IsNullOrWhiteSpace(searchText) || searchText.Length < 3)
        {
            await DisplayAlert("Errore", "Inserisci un nome di città valido (minimo 3 caratteri).", "OK");
            return;
        }

        try
        {
            LoadingIndicator.IsVisible = true;
            LoadingIndicator.IsRunning = true;

            var location = await _addItemViewModel.GetCoordinatesFromCityAsync(searchText);
            if (location != null)
            {
                CityLabel.Text = $"{_addItemViewModel.CityName}, {_addItemViewModel.CountryName}";
                CitySearchEntry.Text = _addItemViewModel.CityName;

                string js = $"updateLocation({_addItemViewModel.Latitude}, {_addItemViewModel.Longitude});";
                Console.WriteLine($"Eseguo: {js}");
                var jsResult = await MapWebView.EvaluateJavaScriptAsync(js);
                Console.WriteLine($"Risultato JS: {jsResult}");
            }
            else
            {
                await DisplayAlert("Errore", "Città non trovata.", "OK");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Errore in OnSearchButtonClicked: {ex.Message}");
            await DisplayAlert("Errore", $"Si è verificato un problema: {ex.Message}", "OK");
        }
        finally
        {
            LoadingIndicator.IsVisible = false;
            LoadingIndicator.IsRunning = false;
        }
    }

    private async void OnSaveButtonClicked(object sender, EventArgs e)
    {
        var city = new City
        {
            Name = _addItemViewModel.CityName,
            Latitude = _addItemViewModel.Latitude,
            Longitude = _addItemViewModel.Longitude,
            Country = _addItemViewModel.CountryName
        };

        await _meteoListViewModel.AddCityAsync(city);
        await Navigation.PopAsync();
    }

    private void OnWebViewNavigated(object sender, WebNavigatedEventArgs e)
    {
        if (e.Result != WebNavigationResult.Success)
        {
            Console.WriteLine($"Errore di navigazione WebView: {e.Result}");
            _ = DisplayAlert("Errore", "Impossibile caricare la mappa.", "OK");
        }
        else
        {
            Console.WriteLine("WebView caricata con successo.");
        }
    }
}