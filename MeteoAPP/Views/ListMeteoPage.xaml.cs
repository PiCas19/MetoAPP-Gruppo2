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
                Debug.WriteLine("Pagina: Inizio caricamento città");
                await _viewModel.LoadCitiesAsync();
                Debug.WriteLine($"Pagina: Numero di città: {_viewModel.Cities.Count}");
                
                await _viewModel.LoadCurrentLocationAsync();
                
                if (_viewModel.FilteredCities.Count == 0 && _viewModel.Cities.Count == 0)
                {
                    await DisplayAlert("Attenzione", "Nessuna città disponibile", "OK");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Pagina: Errore nel caricamento delle città: {ex.Message}");
                await DisplayAlert("Errore", "Impossibile caricare le città", "OK");
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
                Debug.WriteLine($"Errore durante la navigazione: {ex.Message}");
                await DisplayAlert("Errore", $"Errore durante la navigazione: {ex.Message}", "OK");
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
                        await DisplayAlert("Errore", "Impossibile caricare i dati meteo", "OK");
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
                    Debug.WriteLine($"Errore durante la navigazione: {ex.Message}");
                    await DisplayAlert("Errore", $"Errore durante la navigazione: {ex.Message}", "OK");
                }
            }
        }

        private async void OnDeleteItemInvoked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is City city)
            {
                bool confirm = await DisplayAlert("Conferma", $"Vuoi eliminare {city.Name}?", "Sì", "No");
                if (confirm)
                {
                    try
                    {
                        await _viewModel.RemoveCityAsync(city);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Errore durante l'eliminazione della città: {ex.Message}");
                        await DisplayAlert("Errore", $"Errore durante l'eliminazione: {ex.Message}", "OK");
                    }
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
                        await DisplayAlert("Errore", "Impossibile caricare i dati meteo", "OK");
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
                Debug.WriteLine($"Errore durante il recupero della posizione corrente: {ex.Message}");
                await DisplayAlert("Errore", $"Errore durante il recupero della posizione: {ex.Message}", "OK");
            }
            finally
            {
                LoadingIndicator.IsVisible = false;
                LoadingIndicator.IsRunning = false;
            }
        }
    }
}