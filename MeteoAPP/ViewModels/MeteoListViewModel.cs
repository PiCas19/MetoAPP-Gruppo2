using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using MeteoAPP.Models;
using MeteoAPP.Services;

namespace MeteoAPP.ViewModels
{
    /// <summary>
    /// ViewModel che gestisce la logica per la lista delle città monitorate e il meteo associato.
    /// Integra ricerca, eliminazione, aggiunta e sincronizzazione delle città.
    /// </summary>
    public class MeteoListViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;
        private readonly GeoLocationService _locationService;
        private readonly OpenWeatherService _weatherService;
        private ObservableCollection<City> _cities;
        private ObservableCollection<City> _filteredCities;
        private bool _isLoading;
        private string? _currentCityName;
        private string _searchText;
        /// <summary>
        /// Lista completa delle città salvate localmente.
        /// </summary>
        public ObservableCollection<City> Cities
        {
            get => _cities;
            set
            {
                _cities = value;
                OnPropertyChanged();
                UpdateFilteredCities(); 
            }
        }
        /// <summary>
        /// Lista filtrata dinamicamente in base al testo di ricerca.
        /// </summary>
        public ObservableCollection<City> FilteredCities
        {
            get => _filteredCities;
            set
            {
                _filteredCities = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// Indica se è in corso un'operazione di caricamento.
        /// </summary>
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// Nome della città corrente ottenuto tramite geolocalizzazione.
        /// </summary>
        public string CurrentCityName
        {
            get => _currentCityName!;
            set
            {
                _currentCityName = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// Testo inserito nella barra di ricerca per filtrare le città.
        /// </summary>
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                UpdateFilteredCities(); // Aggiorna la lista filtrata quando cambia il testo di ricerca
            }
        }
        /// <summary>
        /// Comando per eliminare una città dalla lista.
        /// </summary>
        public ICommand DeleteCityCommand { get; }
        /// <summary>
        /// Comando per ricaricare la lista delle città.
        /// </summary>
        public ICommand RefreshCommand { get; }

        /// <summary>
        /// Costruttore del ViewModel.
        /// </summary>
        /// <param name="locationService">Servizio per ottenere la posizione geografica.</param>
        /// <param name="weatherService">Servizio per accedere ai dati meteo.</param>
        public MeteoListViewModel(GeoLocationService locationService, OpenWeatherService weatherService)
        {
            _locationService = locationService ?? throw new ArgumentNullException(nameof(locationService));
            _weatherService = weatherService ?? throw new ArgumentNullException(nameof(weatherService));
            _databaseService = App.DatabaseService ?? throw new ArgumentNullException(nameof(App.DatabaseService));
            _cities = new ObservableCollection<City>();
            _filteredCities = new ObservableCollection<City>();
            _currentCityName = "Loading..."; 
            _searchText = string.Empty;

            DeleteCityCommand = new Command<City>(async (city) => await DeleteCityAsync(city));
            RefreshCommand = new Command(async () => await RefreshCitiesAsync());
        }

        /// <summary>
        /// Aggiorna la lista delle città filtrate in base al testo inserito.
        /// </summary>
        private void UpdateFilteredCities()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                FilteredCities = new ObservableCollection<City>(Cities);
            }
            else
            {
                var filtered = Cities.Where(city => 
                    city.Name?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) == true ||
                    city.Country?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) == true).ToList();
                FilteredCities = new ObservableCollection<City>(filtered);
            }
        }

        /// <summary>
        /// Mostra conferma all’utente prima di eliminare una città.
        /// </summary>
        /// <param name="city">La città da eliminare.</param>
        private async Task DeleteCityAsync(City city)
        {
            if (city == null) return;

            try
            {
                bool confirm = await Application.Current!.Windows[0].Page!.DisplayAlert(
                    "Confirm",
                    $"Do you want to eliminate {city.Name}?",
                    "Yes",
                    "No");

                if (confirm)
                {
                    await RemoveCityAsync(city);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Impossible to eliminate the city: {ex.Message}");
                await Application.Current!.Windows[0].Page!.DisplayAlert(
                    "Error",
                    "Impossible to eliminate the city",
                    "OK");
            }
        }
        /// <summary>
        /// Carica tutte le città dal database locale.
        /// </summary>
        public async Task LoadCitiesAsync()
        {
            try
            {
                IsLoading = true;
                var cities = await _databaseService.GetAllCityAsync();
                
                if (cities == null || !cities.Any())
                {
                    Cities = new ObservableCollection<City>();
                }
                else
                {
                    Cities = new ObservableCollection<City>(cities);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"City loading error: {ex.Message}");
                Cities = new ObservableCollection<City>();
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// Ricarica le città dal database.
        /// </summary>
        private async Task RefreshCitiesAsync()
        {
            await LoadCitiesAsync();
        }

        /// <summary>
        /// Aggiunge una nuova città alla lista e la salva localmente.
        /// </summary>
        /// <param name="city">La città da aggiungere.</param>
        public async Task AddCityAsync(City city)
        {
            try
            {
                if (Cities.Any(c => c.Name == city.Name && c.Country == city.Country))
                {
                    await Application.Current!.Windows[0].Page!.DisplayAlert(
                        "Notice", 
                        "This city is already listed.", 
                        "OK");
                    return;
                }

                await _databaseService.AddCityAsync(city);
                await LoadCitiesAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error while adding city: {ex.Message}");
                await Application.Current!.Windows[0].Page!.DisplayAlert(
                    "Error", 
                    $"Unable to add city", 
                    "OK");
            }
        }

        /// <summary>
        /// Rimuove una città dalla lista e la elimina anche da Appwrite.
        /// </summary>
        /// <param name="city">La città da rimuovere.</param>
        public async Task RemoveCityAsync(City city)
        {
            if (city != null)
            {
                try
                {
                    await _databaseService.DeleteCityAsync(city.Id);
                    if (App.AppwriteSyncService != null)
                    {
                        await App.AppwriteSyncService.DeleteCityFromAppwriteAsync(city.Id);
                    }
                    await LoadCitiesAsync();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error during city deletion: {ex.Message}");
                    await Application.Current!.Windows[0].Page!.DisplayAlert(
                        "Error", 
                        $"Unable to delete the city", 
                        "OK"
                    );
                }
            }
        }
        
        /// <summary>
        /// Carica e aggiorna il nome della città corrente tramite GPS.
        /// </summary>
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
                        CurrentCityName = weather.Location ?? "Position unknown"; 
                    }
                    else
                    {
                        CurrentCityName = "Position unknown"; 
                    }
                }
                else
                {
                    CurrentCityName = "Position unknown"; 
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error while loading the current position: {ex.Message}");
                CurrentCityName = "Error";
            }
        }
    }
}