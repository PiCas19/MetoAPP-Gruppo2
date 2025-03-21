namespace MeteoApp;
using System;
using System.ComponentModel;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices.Sensors;
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
            locationName = Uri.UnescapeDataString(value);
            Console.WriteLine($"Set LocationName: {locationName}");
            OnPropertyChanged();
        }
    }
    public AddItemPage()
    {
        InitializeComponent();
        BindingContext = this;
        _ = GetCurrentLocation();
    }
    private async Task GetCurrentLocation()
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
                    await DisplayAlert("Permission Denied", "Location access is required to show your position.", "OK");
                    LoadingIndicator.IsVisible = false;
                    LoadingIndicator.IsRunning = false;
                    return;
                }
            }
            var location = await Geolocation.GetLocationAsync(new GeolocationRequest(GeolocationAccuracy.Best));
            if (location != null)
            {
                latitude = location.Latitude;
                longitude = location.Longitude;
                UpdateMap(latitude, longitude);
                LocationName = await GetLocationNameAsync(latitude, longitude);
                NameEntry.Text = LocationName;
            }

            LoadingIndicator.IsVisible = false;
            LoadingIndicator.IsRunning = false;
        }
        catch (Exception ex)
        {
            LoadingIndicator.IsVisible = false;
            LoadingIndicator.IsRunning = false;
            await DisplayAlert("Error", $"Could not get location: {ex.Message}", "OK");
        }
    }
    private void UpdateMap(double latitude, double longitude)
    {
        var mapUrl = $"https://www.openstreetmap.org/export/embed.html?bbox={longitude-0.01},{latitude-0.01},{longitude+0.01},{latitude+0.01}&layer=mapnik&marker={latitude},{longitude}";
        MapView.Source = mapUrl;
    }
    private async Task<string> GetLocationNameAsync(double latitude, double longitude)
    {
        var url = $"https://nominatim.openstreetmap.org/reverse?format=json&lat={latitude}&lon={longitude}";
        using (var client = new HttpClient())
        {
            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<JsonElement>(json);
                if (data.TryGetProperty("display_name", out JsonElement displayName))
                {
                    return displayName.GetString();
                }
            }
        }
        return "Location not found";
    }
}