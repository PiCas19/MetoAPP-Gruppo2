using System.Diagnostics;
using MeteoAPP.Models;
using MeteoAPP.Services;

namespace MeteoAPP
{
    [QueryProperty(nameof(CityName), "CityName")]
    [QueryProperty(nameof(Temperature), "Temperature")]
    [QueryProperty(nameof(TemperatureMin), "TemperatureMin")]
    [QueryProperty(nameof(TemperatureMax), "TemperatureMax")]
    [QueryProperty(nameof(Description), "Description")]
    [QueryProperty(nameof(Icon), "Icon")]
    [QueryProperty(nameof(WindSpeed), "WindSpeed")]
    [QueryProperty(nameof(RainChance), "RainChance")]
    [QueryProperty(nameof(Pressure), "Pressure")]
    [QueryProperty(nameof(MorningTemp), "MorningTemp")]
    [QueryProperty(nameof(AfternoonTemp), "AfternoonTemp")]
    [QueryProperty(nameof(EveningTemp), "EveningTemp")]
    [QueryProperty(nameof(NightTemp), "NightTemp")]
    public partial class MeteoItemPage : ContentPage
    {

        private readonly IParameterService _parameterService;
        private string? _cityName;
        private string? _temperature;
        private string? _temperatureMin;
        private string? _temperatureMax;
        private string? _description;
        private string? _icon;
        private string? _windSpeed;
        private string? _rainChance;
        private string? _pressure;
        private string? _morningTemp;
        private string? _afternoonTemp;
        private string? _eveningTemp;
        private string? _nightTemp;

        public MeteoItemPage(IParameterService parameterService)
        {
            InitializeComponent();
            _parameterService = parameterService;
        }

        public string? CityName
        {
            get => _cityName;
            set
            {
                _cityName = value;
                UpdateUI();
            }
        }

        public string? Temperature
        {
            get => _temperature;
            set
            {
                _temperature = value;
                UpdateUI();
            }
        }

        public string? TemperatureMin
        {
            get => _temperatureMin;
            set
            {
                _temperatureMin = value;
                UpdateUI();
            }
        }

        public string? TemperatureMax
        {
            get => _temperatureMax;
            set
            {
                _temperatureMax = value;
                UpdateUI();
            }
        }

        public string? Description
        {
            get => _description;
            set
            {
                _description = value;
                UpdateUI();
            }
        }

        public string? Icon
        {
            get => _icon;
            set
            {
                _icon = value;
                UpdateUI();
            }
        }
        public string? WindSpeed
        {
            get => _windSpeed;
            set
            {
                _windSpeed = value;
                UpdateUI();
            }
        }
        public string? RainChance
        {
            get => _rainChance;
            set
            {
                _rainChance = value;
                UpdateUI();
            }
        }
        public string? Pressure
        {
            get => _pressure;
            set
            {
                _pressure = value;
                UpdateUI();
            }
        }
        public string? MorningTemp
        {
            get => _morningTemp;
            set
            {
                _morningTemp = value;
                UpdateUI();
            }
        }
        public string? AfternoonTemp
        {
            get => _afternoonTemp;
            set
            {
                _afternoonTemp = value;
                UpdateUI();
            }
        }
        public string? EveningTemp
        {
            get => _eveningTemp;
            set
            {
                _eveningTemp = value;
                UpdateUI();
            }
        }

        public string? NightTemp
        {
            get => _nightTemp;
            set
            {
                _nightTemp = value;
                UpdateUI();
            }
        }

        private void UpdateUI()
        {
            try
            {
                if (CityNameLabel == null || WeatherLabel == null || TemperatureLabel == null || TemperatureMinLabel == null || TemperatureMaxLabel == null || WeatherIcon == null)
                {
                    return;
                }

                CityNameLabel.Text = _cityName ?? "N/A";
                WeatherLabel.Text = _description ?? "N/A";
                TemperatureLabel.Text = _temperature != null ? $"{_temperature}°" : "N/A";
                TemperatureMinLabel.Text = _temperatureMin != null ? $"{_temperatureMin}°" : "N/A";
                TemperatureMaxLabel.Text = _temperatureMax != null ? $"{_temperatureMax}°" : "N/A";
                WeatherIcon.Source = _icon != null ? $"https://openweathermap.org/img/wn/{_icon}@2x.png" : "https://openweathermap.org/img/wn/01d@2x.png";
                UpdateBackground();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in UpdateUI: {ex.Message}");
                Shell.Current.DisplayAlert("Error", "Unable to update weather data", "OK");
            }
        }

        private void UpdateBackground()
        {
            if (_description == null) return;

            LinearGradientBrush backgroundBrush = new LinearGradientBrush { EndPoint = new Point(0, 1) };
            if (_description.ToLower().Contains("clear"))
            {
                backgroundBrush.GradientStops.Add(new GradientStop { Color = Color.FromArgb("#FFD700"), Offset = 0.0f }); 
                backgroundBrush.GradientStops.Add(new GradientStop { Color = Color.FromArgb("#C2E9FB"), Offset = 1.0f }); 
            }
            else if (_description.ToLower().Contains("cloud"))
            {
                backgroundBrush.GradientStops.Add(new GradientStop { Color = Color.FromArgb("#B0C4DE"), Offset = 0.0f });
                backgroundBrush.GradientStops.Add(new GradientStop { Color = Color.FromArgb("#C2E9FB"), Offset = 1.0f }); 
            }
            else if (_description.ToLower().Contains("rain") || _description.ToLower().Contains("drizzle"))
            {
                backgroundBrush.GradientStops.Add(new GradientStop { Color = Color.FromArgb("#A1C4FD"), Offset = 0.0f }); 
                backgroundBrush.GradientStops.Add(new GradientStop { Color = Color.FromArgb("#C2E9FB"), Offset = 1.0f }); 
            }
            else
            {
                backgroundBrush.GradientStops.Add(new GradientStop { Color = Color.FromArgb("#A1C4FD"), Offset = 0.0f });
                backgroundBrush.GradientStops.Add(new GradientStop { Color = Color.FromArgb("#C2E9FB"), Offset = 1.0f });
            }

            Background = backgroundBrush;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            UpdateUI();
             _parameterService.SetData(new WeatherData
            {
                Location = _cityName ?? "N/A",
                Description = _description ?? "N/A",
                IconCode = _icon ?? "01d",
                Temperature = double.TryParse(_temperature, out var temp) ? temp : 0,
                TemperatureMin = double.TryParse(_temperatureMin, out var tempMin) ? tempMin : 0,
                TemperatureMax = double.TryParse(_temperatureMax, out var tempMax) ? tempMax : 0,
                WindSpeedKmh = double.TryParse(_windSpeed, out var wind) ? wind : 0,
                RainChancePercent = double.TryParse(_rainChance, out var rain) ? rain : 0,
                PressureHpa = double.TryParse(_pressure, out var pressure) ? pressure : 0,
                MorningTemp = double.TryParse(_morningTemp, out var m) ? m : 0,
                AfternoonTemp = double.TryParse(_afternoonTemp, out var a) ? a : 0,
                EveningTemp = double.TryParse(_eveningTemp, out var e) ? e : 0,
                NightTemp = double.TryParse(_nightTemp, out var n) ? n : 0
            });
        }

        private async void OnCityLabelTapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new BlazorPage());
        }

    }
}