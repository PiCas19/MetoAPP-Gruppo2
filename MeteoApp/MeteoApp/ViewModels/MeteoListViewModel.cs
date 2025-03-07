using System.Collections.ObjectModel;
using MeteoApp.Models;


namespace MeteoApp
{
    public class MeteoListViewModel : BaseViewModel
    {
        ObservableCollection<WeatherData> _weatherDataList;

        public ObservableCollection<WeatherData> WeatherDataList
        {
            get { return _weatherDataList; }
            set
            {
                _weatherDataList = value;
                OnPropertyChanged();
            }
        }

        public MeteoListViewModel()
        {
            WeatherDataList = new ObservableCollection<WeatherData>
            {
                new WeatherData
                {
                    Location = new WeatherLocation { Name = "Londra", Lon = -0.13, Lat = 51.51 },
                    Weather = new WeatherInfo { Id = 800, Main = "Clear", Description = "Cielo sereno", Icon = "01d" },
                    Temperature = new TemperatureInfo { Current = 22, FeelsLike = 21, Min = 18, Max = 24, Pressure = 1012, Humidity = 60 },
                    Wind = new WindInfo { Speed = 3.2, Deg = 180, Gust = 4.5 },
                    Cloud = new CloudInfo { All = 5 },
                    Rain = new RainInfo { OneHour = null },
                    Snow = new SnowInfo { OneHour = null },
                    Sunrise = 1678147200,
                    Sunset = 1678190400,
                    Timezone = 3600
                },
                new WeatherData
                {
                    Location = new WeatherLocation { Name = "Roma", Lon = 12.49, Lat = 41.89 },
                    Weather = new WeatherInfo { Id = 500, Main = "Rain", Description = "Pioggia leggera", Icon = "10d" },
                    Temperature = new TemperatureInfo { Current = 18, FeelsLike = 17, Min = 16, Max = 20, Pressure = 1015, Humidity = 75 },
                    Wind = new WindInfo { Speed = 5.1, Deg = 220, Gust = 7.0 },
                    Cloud = new CloudInfo { All = 90 },
                    Rain = new RainInfo { OneHour = 0.8 },
                    Snow = new SnowInfo { OneHour = null },
                    Sunrise = 1678148200,
                    Sunset = 1678192400,
                    Timezone = 3600
                },
                new WeatherData
                {
                    Location = new WeatherLocation { Name = "Parigi", Lon = 2.35, Lat = 48.86 },
                    Weather = new WeatherInfo { Id = 801, Main = "Clouds", Description = "Parzialmente nuvoloso", Icon = "02d" },
                    Temperature = new TemperatureInfo { Current = 20, FeelsLike = 19, Min = 17, Max = 22, Pressure = 1013, Humidity = 65 },
                    Wind = new WindInfo { Speed = 4.0, Deg = 200, Gust = 5.5 },
                    Cloud = new CloudInfo { All = 40 },
                    Rain = new RainInfo { OneHour = null },
                    Snow = new SnowInfo { OneHour = null },
                    Sunrise = 1678149200,
                    Sunset = 1678193400,
                    Timezone = 3600
                },
                new WeatherData
                {
                    Location = new WeatherLocation {  Name = "New York", Lon = -74.01, Lat = 40.71 },
                    Weather = new WeatherInfo { Id = 802, Main = "Clouds", Description = "Nuvoloso", Icon = "03d" },
                    Temperature = new TemperatureInfo { Current = 16, FeelsLike = 15, Min = 13, Max = 18, Pressure = 1020, Humidity = 70 },
                    Wind = new WindInfo { Speed = 2.8, Deg = 150, Gust = 3.9 },
                    Cloud = new CloudInfo { All = 75 },
                    Rain = new RainInfo { OneHour = null },
                    Snow = new SnowInfo { OneHour = null },
                    Sunrise = 1678145200,
                    Sunset = 1678191400,
                    Timezone = -14400
                },
                new WeatherData
                {
                    Location = new WeatherLocation { Name = "Tokyo", Lon = 139.69, Lat = 35.68 },
                    Weather = new WeatherInfo { Id = 600, Main = "Snow", Description = "Nevicata leggera", Icon = "13d" },
                    Temperature = new TemperatureInfo { Current = 2, FeelsLike = -1, Min = 0, Max = 4, Pressure = 1008, Humidity = 85 },
                    Wind = new WindInfo { Speed = 6.0, Deg = 300, Gust = 8.0 },
                    Cloud = new CloudInfo { All = 95 },
                    Rain = new RainInfo { OneHour = null },
                    Snow = new SnowInfo { OneHour = 1.2 },
                    Sunrise = 1678146200,
                    Sunset = 1678190400,
                    Timezone = 32400
                }
            };
        }
    }
}
