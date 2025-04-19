using System.Diagnostics;
using MeteoAPP.Models;
using MeteoAPP.Services;

namespace MeteoAPP
{
    /// <summary>
    /// Pagina che visualizza i dettagli meteo per una città selezionata, con binding tramite query parameters.
    /// </summary>
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

        /// <summary>
        /// Inizializza la pagina con il servizio per il passaggio di dati globali.
        /// </summary>
        /// <param name="parameterService">Servizio per il passaggio dei parametri meteo.</param>
        public MeteoItemPage(IParameterService parameterService)
        {
            InitializeComponent();
            _parameterService = parameterService;
        }

        /// <summary>
        /// Nome della città selezionata.
        /// </summary>
        public string? CityName
        {
            get => _cityName;
            set
            {
                _cityName = value;
                UpdateUI();
            }
        }

        /// <summary>
        /// Temperatura attuale.
        /// </summary>
        public string? Temperature
        {
            get => _temperature;
            set
            {
                _temperature = value;
                UpdateUI();
            }
        }

        /// <summary>
        /// Temperatura minima.
        /// </summary>
        public string? TemperatureMin
        {
            get => _temperatureMin;
            set
            {
                _temperatureMin = value;
                UpdateUI();
            }
        }

        /// <summary>
        /// Temperatura massima.
        /// </summary>
        public string? TemperatureMax
        {
            get => _temperatureMax;
            set
            {
                _temperatureMax = value;
                UpdateUI();
            }
        }

        /// <summary>
        /// Descrizione delle condizioni meteo (es. "nuvoloso").
        /// </summary>
        public string? Description
        {
            get => _description;
            set
            {
                _description = value;
                UpdateUI();
            }
        }

        /// <summary>
        /// Codice dell'icona meteo OpenWeatherMap.
        /// </summary>
        public string? Icon
        {
            get => _icon;
            set
            {
                _icon = value;
                UpdateUI();
            }
        }

        /// <summary>
        /// Velocità del vento in km/h.
        /// </summary>
        public string? WindSpeed
        {
            get => _windSpeed;
            set
            {
                _windSpeed = value;
                UpdateUI();
            }
        }

        /// <summary>
        /// Probabilità di pioggia in percentuale.
        /// </summary>
        public string? RainChance
        {
            get => _rainChance;
            set
            {
                _rainChance = value;
                UpdateUI();
            }
        }

        /// <summary>
        /// Pressione atmosferica in hPa.
        /// </summary>
        public string? Pressure
        {
            get => _pressure;
            set
            {
                _pressure = value;
                UpdateUI();
            }
        }

        /// <summary>
        /// Temperatura del mattino.
        /// </summary>
        public string? MorningTemp
        {
            get => _morningTemp;
            set
            {
                _morningTemp = value;
                UpdateUI();
            }
        }

        /// <summary>
        /// Temperatura del pomeriggio.
        /// </summary>
        public string? AfternoonTemp
        {
            get => _afternoonTemp;
            set
            {
                _afternoonTemp = value;
                UpdateUI();
            }
        }

        /// <summary>
        /// Temperatura della sera.
        /// </summary>
        public string? EveningTemp
        {
            get => _eveningTemp;
            set
            {
                _eveningTemp = value;
                UpdateUI();
            }
        }

        /// <summary>
        /// Temperatura della notte.
        /// </summary>
        public string? NightTemp
        {
            get => _nightTemp;
            set
            {
                _nightTemp = value;
                UpdateUI();
            }
        }

        /// <summary>
        /// Aggiorna la UI in base ai dati meteo correnti.
        /// </summary>
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

        /// <summary>
        /// Cambia dinamicamente lo sfondo della pagina in base alla descrizione meteo.
        /// </summary>
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

        
        /// <summary>
        /// Evento chiamato alla visualizzazione della pagina. Sincronizza i dati col parametro service.
        /// </summary>
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

        /// <summary>
        /// Naviga alla pagina Blazor (grafici) quando si clicca sul nome della città.
        /// </summary>
        /// <param name="sender">L'oggetto che ha generato l'evento (in questo caso un'etichetta tappata).</param>
        /// <param name="e">I dati dell'evento di tap associato all'interazione utente.</param>
        private async void OnCityLabelTapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new BlazorPage());
        }

    }
}