using System.Collections.ObjectModel;
using MeteoApp.Models;
namespace MeteoApp
{
    public class MeteoListViewModel : BaseViewModel
    {
        private ObservableCollection<WeatherData> _weatherDataList;
        
        public ObservableCollection<WeatherData> WeatherDataList
        {
            get => _weatherDataList;
            set
            {
                _weatherDataList = value;
                OnPropertyChanged();
            }
        }
        
        public MeteoListViewModel()
        {
            _weatherDataList = new ObservableCollection<WeatherData>();
        }
    }
}
