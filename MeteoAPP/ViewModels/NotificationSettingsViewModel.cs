using System.Text;
using System.Text.Json;
using MeteoAPP.Models;
using Plugin.Firebase.CloudMessaging;

namespace MeteoAPP.ViewModels
{
    public class NotificationSettingsViewModel : BaseViewModel
    {
        private City? _city;
        private double _highTemperatureThreshold = 30.0;
        private double _lowTemperatureThreshold = 10.0;
        private bool _isNotificationEnabled = true;
        private readonly HttpClient _httpClient;
        private string _baseApiUrl = "";

        public City City
        {
            get => _city ?? throw new InvalidOperationException("City cannot be null");
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
            set
            {
                if (SetProperty(ref _isNotificationEnabled, value))
                {
                    SaveNotificationPreference();
                }
            }
        }

        public NotificationSettingsViewModel(City city, HttpClient httpClient)
        {
            City = city ?? throw new ArgumentNullException(nameof(city));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _ = LoadBaseUrlAsync();
            LoadSettings();
        }

        private async Task LoadBaseUrlAsync()
        {
            try
            {
                var path = Path.Combine(FileSystem.AppDataDirectory, "config.json");
                var json = await File.ReadAllTextAsync(path);
                var config = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
                _baseApiUrl = config?["NotificationSettingsBaseUrl"]?.TrimEnd('/') ?? "";
            }
            catch (Exception ex)
            {
                Console.WriteLine("Errore nel caricamento del base URL: " + ex.Message);
            }
        }

        private string GeneratePreferenceKey(string token, string location)
        {
            return $"notifications_enabled_{token}_{location}";
        }

        private async void LoadSettings()
        {
            try
            {
                if (string.IsNullOrEmpty(_baseApiUrl))
                    await LoadBaseUrlAsync();

                string token = "";
                try
                {
                    await CrossFirebaseCloudMessaging.Current.CheckIfValidAsync();
                    token = await CrossFirebaseCloudMessaging.Current.GetTokenAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Errore ottenendo il token Firebase: " + ex.Message);
                }

                var endpoint = $"{_baseApiUrl}/api/NotificationSettings";
                var builder = new UriBuilder(endpoint);
                var query = System.Web.HttpUtility.ParseQueryString(builder.Query);
                query["token"] = token;
                query["location"] = $"{City.Name}, {City.Country}";
                builder.Query = query.ToString();
                var url = builder.ToString();

                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var settingsList = JsonSerializer.Deserialize<List<UserNotificationSettings>>(content);
                    if (settingsList?.Count > 0)
                    {
                        var settings = settingsList[0];
                        HighTemperatureThreshold = settings.TemperatureMax;
                        LowTemperatureThreshold = settings.TemperatureMin;
                        var preferenceKey = GeneratePreferenceKey(token, settings.Location!);
                        IsNotificationEnabled = Preferences.Get(preferenceKey, true);
                    }
                }
                else
                {
                    Console.WriteLine("Errore nel caricamento: " + response.ReasonPhrase);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Errore LoadSettings(): " + ex.Message);
            }
        }

        private void SaveNotificationPreference()
        {
            try
            {
                var token = Preferences.Get("firebase_token", "");
                var location = $"{City.Name}, {City.Country}";
                var preferenceKey = GeneratePreferenceKey(token, location);
                Preferences.Set(preferenceKey, IsNotificationEnabled);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Errore nel salvataggio preferenza: " + ex.Message);
            }
        }

        public async Task SaveSettingsAsync()
        {
            try
            {
                if (IsNotificationEnabled && HighTemperatureThreshold <= LowTemperatureThreshold)
                {
                    throw new InvalidOperationException("Soglia alta deve essere maggiore della bassa.");
                }

                string token = "";
                try
                {
                    await CrossFirebaseCloudMessaging.Current.CheckIfValidAsync();
                    token = await CrossFirebaseCloudMessaging.Current.GetTokenAsync();
                    Preferences.Set("firebase_token", token);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Errore ottenendo token: " + ex.Message);
                }

                var settings = new UserNotificationSettings
                {
                    Token = token,
                    Location = $"{City.Name}, {City.Country}",
                    TemperatureMax = HighTemperatureThreshold,
                    TemperatureMin = LowTemperatureThreshold,
                    IsEnabled = IsNotificationEnabled
                };

                var settingsList = new List<UserNotificationSettings> { settings };
                var content = new StringContent(JsonSerializer.Serialize(settingsList), Encoding.UTF8, "application/json");

                if (string.IsNullOrEmpty(_baseApiUrl))
                    await LoadBaseUrlAsync();

                var endpoint = $"{_baseApiUrl}/api/NotificationSettings";
                var response = await _httpClient.PostAsync(endpoint, content);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("✅ Impostazioni salvate.");
                }
                else
                {
                    Console.WriteLine("❌ Errore salvataggio: " + response.ReasonPhrase);
                }

                SaveNotificationPreference();
                OnPropertyChanged(nameof(HighTemperatureThreshold));
                OnPropertyChanged(nameof(LowTemperatureThreshold));
                OnPropertyChanged(nameof(IsNotificationEnabled));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Errore SaveSettingsAsync(): " + ex.Message);
                throw;
            }
        }
    }
}
