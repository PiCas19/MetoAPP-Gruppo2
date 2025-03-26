using System.Diagnostics;
using Microsoft.Maui.Controls;

namespace MeteoAPP
{
    [QueryProperty(nameof(CityName), "CityName")]
    [QueryProperty(nameof(Temperature), "Temperature")]
    [QueryProperty(nameof(TemperatureMin), "TemperatureMin")]
    [QueryProperty(nameof(TemperatureMax), "TemperatureMax")]
    [QueryProperty(nameof(Description), "Description")]
    [QueryProperty(nameof(Icon), "Icon")]

    public partial class MeteoItemPage : ContentPage
    {
        private string? _cityName;
        private string? _temperature;
        private string? _temperatureMin;
        private string? _temperatureMax;
        private string? _description;
        private string? _icon;

        public MeteoItemPage()
        {
            InitializeComponent();
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

        private void UpdateUI()
        {
            try
            {
                if (CityNameLabel == null || WeatherLabel == null || TemperatureLabel == null || TemperatureMinLabel == null || TemperatureMaxLabel == null || WeatherIcon == null)
                {
                    Debug.WriteLine("Errore: Uno o più controlli UI non sono inizializzati.");
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
                Debug.WriteLine($"Errore in UpdateUI: {ex.Message}");
                Shell.Current.DisplayAlert("Errore", "Impossibile aggiornare i dati meteo", "OK");
            }
        }

        private void UpdateBackground()
        {
            if (_description == null) return;

            LinearGradientBrush backgroundBrush = new LinearGradientBrush { EndPoint = new Point(0, 1) };
            if (_description.ToLower().Contains("clear"))
            {
                backgroundBrush.GradientStops.Add(new GradientStop { Color = Color.FromArgb("#FFD700"), Offset = 0.0f }); // Giallo chiaro
                backgroundBrush.GradientStops.Add(new GradientStop { Color = Color.FromArgb("#C2E9FB"), Offset = 1.0f }); // Celeste chiaro
            }
            else if (_description.ToLower().Contains("cloud"))
            {
                backgroundBrush.GradientStops.Add(new GradientStop { Color = Color.FromArgb("#B0C4DE"), Offset = 0.0f }); // Grigio chiaro
                backgroundBrush.GradientStops.Add(new GradientStop { Color = Color.FromArgb("#C2E9FB"), Offset = 1.0f }); // Celeste chiaro
            }
            else if (_description.ToLower().Contains("rain") || _description.ToLower().Contains("drizzle"))
            {
                backgroundBrush.GradientStops.Add(new GradientStop { Color = Color.FromArgb("#A1C4FD"), Offset = 0.0f }); // Blu chiaro
                backgroundBrush.GradientStops.Add(new GradientStop { Color = Color.FromArgb("#C2E9FB"), Offset = 1.0f }); // Celeste chiaro
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
            Debug.WriteLine("MeteoItemPage OnAppearing chiamato");
            UpdateUI();
        }
    }
}