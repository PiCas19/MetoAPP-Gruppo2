using System.Collections.ObjectModel;
using System.Diagnostics;
using MeteoAPP.Models;
using MeteoAPP.Services;

namespace MeteoAPP.ViewModels
{
    public class MeteoListViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;
        private ObservableCollection<City> _cities;

        public ObservableCollection<City> Cities
        {
            get => _cities;
            set
            {
                _cities = value;
                OnPropertyChanged();
            }
        }

        public MeteoListViewModel()
        {
            _databaseService = App.DatabaseService ?? throw new ArgumentNullException(nameof(App.DatabaseService));
            _cities = new ObservableCollection<City>();
        }

        public async Task LoadCitiesAsync()
        {
            try
            {
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
            try
            {
                await _databaseService.DeleteCityAsync(city.Id); 
                Cities.Remove(city); 
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Errore durante la rimozione della città: {ex.Message}");
            }
        }
    }
}