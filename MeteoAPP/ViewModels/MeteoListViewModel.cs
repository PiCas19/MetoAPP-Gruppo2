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
        private ObservableCollection<City> _filteredCities;
        private bool _isLoading;
        private string? _currentCityName;
        private string _searchText;

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

        public ObservableCollection<City> FilteredCities
        {
            get => _filteredCities;
            set
            {
                _filteredCities = value;
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

        public ICommand DeleteCityCommand { get; }
        public ICommand RefreshCommand { get; }

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

        private async Task RefreshCitiesAsync()
        {
            await LoadCitiesAsync();
        }

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

        public async Task RemoveCityAsync(City city)
        {
            if (city != null)
            {
                try
                {
                    await _databaseService.DeleteCityAsync(city.Id);
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