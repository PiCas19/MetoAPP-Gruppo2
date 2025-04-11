using System.Text.Json;
using MeteoAPP.Models;
using System.Net.Http;
using System.Text;
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
            set => SetProperty(ref _isNotificationEnabled, value);
        }

        public NotificationSettingsViewModel(City city, HttpClient httpClient)
        {
            City = city ?? throw new ArgumentNullException(nameof(city), "City cannot be null");
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient), "HttpClient cannot be null");
            LoadSettings();
        }

        private async void LoadSettings()
        {
            try
            {
                var cityKey = $"{City.Name}_{City.Country}";
                var response = await _httpClient.GetAsync($"http://localhost:5000/api/NotificationSettings?cityKey={cityKey}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var settings = JsonSerializer.Deserialize<NotificationSettings>(content);

                    HighTemperatureThreshold = settings?.HighTemperatureThreshold ?? 30.0;
                    LowTemperatureThreshold = settings?.LowTemperatureThreshold ?? 10.0;
                    IsNotificationEnabled = settings?.IsNotificationEnabled ?? true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading settings: " + ex.Message);
            }
        }

        public async Task SaveSettingsAsync()
        {
            try
            {
                // Controllo che la soglia di temperatura alta sia maggiore di quella bassa
                if (IsNotificationEnabled && HighTemperatureThreshold <= LowTemperatureThreshold)
                {
                    throw new InvalidOperationException("High temperature threshold must be greater than low temperature threshold.");
                }

                var cityKey = $"{City.Name}_{City.Country}";
                var settings = new NotificationSettings
                {
                    HighTemperatureThreshold = HighTemperatureThreshold,
                    LowTemperatureThreshold = LowTemperatureThreshold,
                    IsNotificationEnabled = IsNotificationEnabled
                };

                var content = new StringContent(JsonSerializer.Serialize(settings), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("http://localhost:5000/api/NotificationSettings", content);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Settings saved successfully.");

                    string token = string.Empty;
                    try
                    {
                        // Ottenere il token da Firebase Cloud Messaging
                        await CrossFirebaseCloudMessaging.Current.CheckIfValidAsync();
                        token = await CrossFirebaseCloudMessaging.Current.GetTokenAsync();
                        Console.WriteLine("Firebase token obtained: " + token);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error obtaining Firebase token: " + ex.Message);
                        // Gestisci l'errore (ad esempio, il token potrebbe non essere disponibile)
                    }

                    // Crea un oggetto UserNotificationSettings
                    var userNotificationSettings = new UserNotificationSettings
                    {
                        Token = token,  // Aggiungi il token ottenuto
                        Location = $"{City.Name}, {City.Country}",
                        TemperatureMax = HighTemperatureThreshold,
                        TemperatureMin = LowTemperatureThreshold
                    };

                    // Serializza e invia a Firestore
                    var firestoreContent = new StringContent(JsonSerializer.Serialize(userNotificationSettings), Encoding.UTF8, "application/json");
                    var firestoreResponse = await _httpClient.PostAsync("http://localhost:5000/api/UserNotificationSettings", firestoreContent);

                    if (firestoreResponse.IsSuccessStatusCode)
                    {
                        Console.WriteLine("User notification settings saved to Firestore.");
                    }
                    else
                    {
                        Console.WriteLine("Error saving user notification settings: " + firestoreResponse.ReasonPhrase);
                    }

                    await RegisterCityForNotificationsAsync();
                }
                else
                {
                    Console.WriteLine("Error saving settings: " + response.ReasonPhrase);
                }

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



        private async Task RegisterCityForNotificationsAsync()
        {
            await Task.CompletedTask;
            Console.WriteLine($"Registered {City.Name}, {City.Country} for temperature notifications");
        }
    }
}
