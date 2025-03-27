using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using MeteoAPP.Models;
using MeteoAPP.Services;

namespace MeteoAPP.ViewModels
{
    public class MeteoListViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;
        private readonly GeoLocationService _locationService;
        private readonly OpenWeatherService _weatherService;
        private ObservableCollection<City> _cities;
        private bool _isLoading;
        private string? _currentCityName;

        public ObservableCollection<City> Cities
        {
            get => _cities;
            set
            {
                _cities = value;
                OnPropertyChanged();
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        public string CurrentCityName
        {
            get => _currentCityName!;
            set
            {
                _currentCityName = value;
                OnPropertyChanged();
            }
        }

        public ICommand DeleteCityCommand { get; }
        public ICommand RefreshCommand { get; }

        public MeteoListViewModel(GeoLocationService locationService, OpenWeatherService weatherService)
        {
            _locationService = locationService ?? throw new ArgumentNullException(nameof(locationService));
            _weatherService = weatherService ?? throw new ArgumentNullException(nameof(weatherService));
            _databaseService = App.DatabaseService ?? throw new ArgumentNullException(nameof(App.DatabaseService));
            _cities = new ObservableCollection<City>();
            _currentCityName = "Caricamento..."; 

            DeleteCityCommand = new Command<City>(async (city) => await DeleteCityAsync(city));
            RefreshCommand = new Command(async () => await RefreshCitiesAsync());
        }

        private async Task DeleteCityAsync(City city)
        {
            if (city == null) return;

            try
            {
                bool confirm = await Application.Current!.Windows[0].Page!.DisplayAlert(
                    "Conferma",
                    $"Vuoi eliminare {city.Name}?",
                    "Sì",
                    "No");

                if (confirm)
                {
                    await RemoveCityAsync(city);
                }
            }
            catch (Exception ex)
            {
                await Application.Current!.Windows[0].Page!.DisplayAlert(
                    "Errore",
                    "Impossibile eliminare la città: " + ex.Message,
                    "OK");
            }
        }

        public async Task LoadCitiesAsync()
        {
            try
            {
                IsLoading = true;
                Debug.WriteLine("Caricamento delle città in corso...");
                var cities = await _databaseService.GetAllCityAsync();
                
                if (cities == null || !cities.Any())
                {
                    Debug.WriteLine("Nessuna città trovata nel database.");
                    Cities = new ObservableCollection<City>();
                }
                else
                {
                    Cities = new ObservableCollection<City>(cities);
                    Debug.WriteLine($"Caricate {Cities.Count} città.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Errore caricamento città: {ex.Message}");
                Cities = new ObservableCollection<City>();
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task RefreshCitiesAsync()
        {
            await LoadCitiesAsync();
        }

        public async Task AddCityAsync(City city)
        {
            try
            {
                await _databaseService.AddCityAsync(city);
                Cities.Add(city);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Errore durante l'aggiunta della città: {ex.Message}");
            }
        }

        public async Task RemoveCityAsync(City city)
        {
            if (city != null)
            {
                try
                {
                    await _databaseService.DeleteCityAsync(city.Id);
                    Cities.Remove(city);
                    Debug.WriteLine($"Città {city.Name} eliminata con successo.");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Errore durante l'eliminazione della città: {ex.Message}");
                    throw; 
                }
            }
        }

        public async Task LoadCurrentLocationAsync()
        {
            try
            {
                var locationResult = await _locationService.GetCurrentLocationAsync();

                if (locationResult.Success)
                {
                    var weather = await _weatherService.GetWeatherByCoordinatesAsync(
                        locationResult.Latitude,
                        locationResult.Longitude);

                    if (weather != null)
                    {
                        CurrentCityName = weather.Location ?? "Posizione sconosciuta"; 
                    }
                    else
                    {
                        CurrentCityName = "Posizione sconosciuta"; 
                    }
                }
                else
                {
                    CurrentCityName = "Posizione sconosciuta"; 
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Errore durante il caricamento della posizione corrente: {ex.Message}");
                CurrentCityName = "Errore";
            }
        }
    }
}