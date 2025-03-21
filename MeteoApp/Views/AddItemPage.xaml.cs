namespace MeteoApp;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.ApplicationModel;

public partial class AddItemPage : ContentPage, INotifyPropertyChanged
{
    private string locationName;
    private double latitude;
    private double longitude;

    public string LocationName
    {
        get => locationName;
        set
        {
            locationName = value;
            OnPropertyChanged();
        }
    }

    public AddItemPage()
    {
        InitializeComponent();
        BindingContext = this;

#if ANDROID
        MapWebView.Source = "file:///android_asset/map.html"; 
#else
        MapWebView.Source = "map.html"; 
#endif

        MapWebView.Navigating += WebView_Navigating;

        _ = RequestLocationPermissionAndGetLocation();
    }

    private async Task RequestLocationPermissionAndGetLocation()
    {
        try
        {
            LoadingIndicator.IsVisible = true;
            LoadingIndicator.IsRunning = true;

            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                if (status != PermissionStatus.Granted)
                {
                    await DisplayAlert("Permesso negato", "L'accesso alla posizione è necessario per mostrare la tua posizione.", "OK");
                    return;
                }
            }

            await GetCurrentLocation();
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

    private async Task GetCurrentLocation()
    {
        try
        {
            var location = await Geolocation.GetLocationAsync(new GeolocationRequest(GeolocationAccuracy.Best));
            if (location != null)
            {
                latitude = location.Latitude;
                longitude = location.Longitude;

                // Aggiorna l'etichetta delle coordinate per debug
                CoordinatesLabel.Text = $"Coordinate: {latitude:F6}, {longitude:F6}";

                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    string locationName = await GetLocationNameAsync(latitude, longitude);
                    LocationName = locationName;
                    CityLabel.Text = locationName;

                    // Invia la posizione alla WebView
                    string js = $"updateLocation({latitude}, {longitude});";
                    await MapWebView.EvaluateJavaScriptAsync(js);
                });
            }
            else
            {
                await DisplayAlert("Errore", "Impossibile ottenere la posizione.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Errore", $"Impossibile ottenere la posizione: {ex.Message}", "OK");
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

                if (coordinates != null && coordinates.ContainsKey("lat") && coordinates.ContainsKey("lng"))
                {
                    latitude = coordinates["lat"];
                    longitude = coordinates["lng"];
                    
                    // Aggiorna l'etichetta delle coordinate per debug
                    MainThread.BeginInvokeOnMainThread(() => {
                        CoordinatesLabel.Text = $"Coordinate: {latitude:F6}, {longitude:F6}";
                    });

                    string locationName = await GetLocationNameAsync(latitude, longitude);

                    // Aggiorna la UI
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        LocationName = locationName;
                        CityLabel.Text = locationName;
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore durante la deserializzazione: {ex.Message}");
            }
        }
    }

    private async Task<string> GetLocationNameAsync(double latitude, double longitude)
    {
        string url = $"https://nominatim.openstreetmap.org/reverse?format=json&lat={latitude}&lon={longitude}";

        try
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "MeteoApp/1.0");
            var response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<JsonElement>(json);

                if (data.TryGetProperty("address", out JsonElement address))
                {
                    string city = address.TryGetProperty("city", out JsonElement cityElement) ? cityElement.GetString() :
                                address.TryGetProperty("town", out JsonElement townElement) ? townElement.GetString() :
                                address.TryGetProperty("village", out JsonElement villageElement) ? villageElement.GetString() : 
                                address.TryGetProperty("county", out JsonElement countyElement) ? countyElement.GetString() : "Sconosciuto";

                    string country = address.TryGetProperty("country", out JsonElement countryElement) ? countryElement.GetString() : "Sconosciuto";

                    return $"{city}, {country}";
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Errore durante il recupero della posizione: {ex.Message}");
        }

        return "Posizione non trovata";
    }

    private void OnWebViewNavigated(object sender, WebNavigatedEventArgs e)
    {
        if (e.Result != WebNavigationResult.Success)
        {
            Console.WriteLine($"Errore di navigazione WebView: {e.Result}");
            DisplayAlert("Errore", "Impossibile caricare la mappa.", "OK");
        }
        else
        {
            Console.WriteLine("WebView caricata con successo.");
        }
    }
}