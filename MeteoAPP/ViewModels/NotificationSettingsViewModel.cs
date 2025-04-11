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
            City = city ?? throw new ArgumentNullException(nameof(city), "City cannot be null");
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient), "HttpClient cannot be null");
            LoadSettings();
        }

        private string GeneratePreferenceKey(string token, string location)
        {
            return $"notifications_enabled_{token}_{location}";
        }

        private async void LoadSettings()
        {
            try
            {
                string token = string.Empty;
                try
                {
                    await CrossFirebaseCloudMessaging.Current.CheckIfValidAsync();
                    token = await CrossFirebaseCloudMessaging.Current.GetTokenAsync();
                    Console.WriteLine("Token Firebase ottenuto: " + token);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Errore nell'ottenere il token Firebase: " + ex.Message);
                }

                var baseUrl = "https://ec0d-2a02-1210-6029-2600-79cb-5506-4701-cb2b.ngrok-free.app/api/NotificationSettings";
                var builder = new UriBuilder(baseUrl);
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

                    if (settingsList != null && settingsList.Count > 0)
                    {
                        var settings = settingsList[0];
                        HighTemperatureThreshold = settings.TemperatureMax;
                        LowTemperatureThreshold = settings.TemperatureMin;

                        // Retrieve the notification preference
                        var preferenceKey = GeneratePreferenceKey(token, settings.Location!);
                        IsNotificationEnabled = Preferences.Get(preferenceKey, true);
                    }
                    else
                    {
                        HighTemperatureThreshold = 30.0;
                        LowTemperatureThreshold = 10.0;
                        IsNotificationEnabled = false;
                    }
                }
                else
                {
                    Console.WriteLine("Errore nel caricamento delle impostazioni: " + response.ReasonPhrase);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Errore nel caricamento delle impostazioni: " + ex.Message);
            }
        }

        private void SaveNotificationPreference()
        {
            try
            {
                var token = Preferences.Get("firebase_token", string.Empty);
                var location = $"{City.Name}, {City.Country}";
                var preferenceKey = GeneratePreferenceKey(token, location);
                Preferences.Set(preferenceKey, IsNotificationEnabled);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Errore nel salvataggio della preferenza di notifica: " + ex.Message);
            }
        }

        public async Task SaveSettingsAsync()
        {
            try
            {
                // Controllo che la soglia di temperatura alta sia maggiore di quella bassa
                if (IsNotificationEnabled && HighTemperatureThreshold <= LowTemperatureThreshold)
                {
                    throw new InvalidOperationException("La soglia di temperatura alta deve essere maggiore di quella bassa.");
                }

                // Ottenere il token da Firebase Cloud Messaging
                string token = string.Empty;
                try
                {
                    await CrossFirebaseCloudMessaging.Current.CheckIfValidAsync();
                    token = await CrossFirebaseCloudMessaging.Current.GetTokenAsync();
                    Console.WriteLine("Token Firebase ottenuto: " + token);

                    // Save token for later use
                    Preferences.Set("firebase_token", token);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Errore nell'ottenere il token Firebase: " + ex.Message);
                    // Gestisci l'errore (ad esempio, il token potrebbe non essere disponibile)
                }

                // Creare l'oggetto UserNotificationSettings
                var userNotificationSettings = new UserNotificationSettings
                {
                    Token = token,
                    Location = $"{City.Name}, {City.Country}",
                    TemperatureMax = HighTemperatureThreshold,
                    TemperatureMin = LowTemperatureThreshold,
                    IsEnabled = IsNotificationEnabled
                };

                // Serializzare e inviare l'array JSON
                var settingsList = new List<UserNotificationSettings> { userNotificationSettings };
                var content = new StringContent(JsonSerializer.Serialize(settingsList), Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("https://ec0d-2a02-1210-6029-2600-79cb-5506-4701-cb2b.ngrok-free.app/api/NotificationSettings", content);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Impostazioni di notifica utente salvate con successo.");
                }
                else
                {
                    Console.WriteLine("Errore nel salvataggio delle impostazioni di notifica utente: " + response.ReasonPhrase);
                }

                // Save the notification preference
                SaveNotificationPreference();

                // Aggiorna le proprietà nel modello
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
    }
}
