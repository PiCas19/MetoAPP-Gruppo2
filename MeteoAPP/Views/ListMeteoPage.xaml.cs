
using MeteoAPP.ViewModels;
using MeteoAPP.Models;
using MeteoAPP.Services;
using System.Diagnostics;
using MeteoApp;
using System.Text;

namespace MeteoAPP
{
    public partial class ListMeteoPage : ContentPage
    {
        private readonly MeteoListViewModel _viewModel;
        private readonly IWeatherService _weatherService;
        private readonly GeoLocationService _locationService;

        public string SelectedProvider { get; set; }

        public ListMeteoPage(string selectedProvider)
        {
            InitializeComponent();
            _locationService = new GeoLocationService();
            SelectedProvider = selectedProvider;

            if (SelectedProvider.Equals("OpenWeather", StringComparison.OrdinalIgnoreCase))
            {
                _weatherService = new OpenWeatherService();
            }
            else if (SelectedProvider.Equals("WeatherAPI", StringComparison.OrdinalIgnoreCase))
            {
                _weatherService = new WeatherApiService();
            }
            else
            {
                throw new InvalidOperationException("Provider meteo non riconosciuto.");
            }

            _ = _weatherService.InitializeAsync();

            _viewModel = new MeteoListViewModel(_locationService, _weatherService);
            BindingContext = _viewModel;
            ProviderLabel.Text = $"{SelectedProvider}";
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
                Android.Util.Log.Debug("MeteoAPP", "aggiunto");

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
                        { "Icon", weather.IconCode ?? "01d" },
                        { "WindSpeed", weather.WindSpeedKmh.ToString("F0") },
                        { "RainChance", weather.RainChancePercent.ToString("F0") },
                        { "Pressure", weather.PressureHpa.ToString("F0") },
                        { "MorningTemp", weather.MorningTemp.ToString("F0") },
                        { "AfternoonTemp", weather.AfternoonTemp.ToString("F0") },
                        { "EveningTemp", weather.EveningTemp.ToString("F0") },
                        { "NightTemp", weather.NightTemp.ToString("F0") }
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
                    HttpClient httpClient = new HttpClient();
                    var settingsViewModel = new NotificationSettingsViewModel(city, httpClient);
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

        private async void OnExportClicked(object sender, EventArgs e)
        {
            try
            {
                var exportService = new ExportImportService(_viewModel.DatabaseService);
                var path = await exportService.ExportAsync();
                await DisplayAlert("Esporta", $"Backup creato:\n{path}", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Errore", $"Esportazione fallita: {ex.Message}", "OK");
            }
        }

        private async void OnImportClicked(object sender, EventArgs e)
        {
            try
            {
                var exportService = new ExportImportService(_viewModel.DatabaseService);

                // Percorso predefinito nella cartella locale
                var path = Path.Combine(FileSystem.AppDataDirectory, "meteoapp-backup.json");

                if (!File.Exists(path))
                {
                    await DisplayAlert("Importa", "Nessun backup trovato nella cartella locale. Effettua prima un'export.", "OK");
                    return;
                }

                // Leggi il JSON e mostralo in anteprima
                var json = await File.ReadAllTextAsync(path, Encoding.UTF8);
                var preview = TryPrettyJson(json, maxChars: 1800);

                bool conferma = await DisplayAlert("Anteprima backup JSON", preview, "Importa", "Annulla");
                if (!conferma)
                    return;

                // Se confermato → Importa con validazione
                await exportService.ImportAsync();
                await _viewModel.LoadCitiesAsync();
                await DisplayAlert("Importa", "Dati importati correttamente dal backup locale.", "OK");
            }
            catch (InvalidDataException ex)
            {
                await DisplayAlert("Validazione fallita", ex.Message, "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Errore", $"Importazione fallita: {ex.Message}", "OK");
            }
        }
        private static string TryPrettyJson(string raw, int maxChars)
        {
            try
            {
                using var doc = System.Text.Json.JsonDocument.Parse(raw);
                var pretty = System.Text.Json.JsonSerializer.Serialize(doc.RootElement, new System.Text.Json.JsonSerializerOptions
                {
                    WriteIndented = true
                });

                return pretty.Length > maxChars
                    ? pretty.Substring(0, maxChars) + "\n… (contenuto troncato)"
                    : pretty;
            }
            catch
            {
                // Se non è JSON valido mostriamo comunque il testo grezzo
                return raw.Length > maxChars
                    ? raw.Substring(0, maxChars) + "\n… (contenuto troncato)"
                    : raw;
            }
        }
    }
}
