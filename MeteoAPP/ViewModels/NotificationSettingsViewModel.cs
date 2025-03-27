using System.Text.Json;
using MeteoAPP.Models;

namespace MeteoAPP.ViewModels
{
    public class NotificationSettingsViewModel : BaseViewModel
    {
        private City? _city;
        private double _highTemperatureThreshold = 30.0; 
        private double _lowTemperatureThreshold = 10.0;  
        private bool _isNotificationEnabled = true;

        public City City 
        { 
            get => _city!; 
            set => SetProperty(ref _city, value); 
        }

        public double HighTemperatureThreshold
        {
            get => _highTemperatureThreshold;
            set => SetProperty(ref _highTemperatureThreshold, value);
        }

        public double LowTemperatureThreshold
        {
            get => _lowTemperatureThreshold;
            set => SetProperty(ref _lowTemperatureThreshold, value);
        }

        public bool IsNotificationEnabled
        {
            get => _isNotificationEnabled;
            set => SetProperty(ref _isNotificationEnabled, value);
        }

        public NotificationSettingsViewModel(City city)
        {
            City = city;
            LoadSettings();
        }

        private void LoadSettings()
        {
            string cityKey = $"{City.Name}_{City.Country}";
            HighTemperatureThreshold = Preferences.Get($"{cityKey}_HighTempThreshold", 30.0);
            LowTemperatureThreshold = Preferences.Get($"{cityKey}_LowTempThreshold", 10.0);
            IsNotificationEnabled = Preferences.Get($"{cityKey}_NotificationEnabled", true);
        }

        public async Task SaveSettingsAsync()
        {
            try
            {
                if (IsNotificationEnabled && HighTemperatureThreshold <= LowTemperatureThreshold)
                {
                    throw new InvalidOperationException("High temperature threshold must be greater than low temperature threshold.");
                }

                string cityKey = $"{City.Name}_{City.Country}";

                Preferences.Set($"{cityKey}_HighTempThreshold", HighTemperatureThreshold);
                Preferences.Set($"{cityKey}_LowTempThreshold", LowTemperatureThreshold);
                Preferences.Set($"{cityKey}_NotificationEnabled", IsNotificationEnabled);

                await RegisterCityForNotificationsAsync();
                OnPropertyChanged(nameof(HighTemperatureThreshold));
                OnPropertyChanged(nameof(LowTemperatureThreshold));
                OnPropertyChanged(nameof(IsNotificationEnabled));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        private async Task RegisterCityForNotificationsAsync()
        {

            await Task.CompletedTask;
            Console.WriteLine($"Registered {City.Name}, {City.Country} for temperature notifications");
        }
    }
}