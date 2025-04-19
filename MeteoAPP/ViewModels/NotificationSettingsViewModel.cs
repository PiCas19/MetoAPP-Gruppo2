using System.Text;
using System.Text.Json;
using MeteoAPP.Models;
using Plugin.Firebase.CloudMessaging;
using MeteoAPP.Services;

namespace MeteoAPP.ViewModels
{
    /// <summary>
    /// ViewModel per la gestione delle impostazioni di notifica personalizzate per ciascuna città.
    /// Include soglie di temperatura e preferenze utente.
    /// </summary>
    public class NotificationSettingsViewModel : BaseViewModel
    {
        private City? _city;
        private double _highTemperatureThreshold = 30.0;
        private double _lowTemperatureThreshold = 10.0;
        private bool _isNotificationEnabled = true;
        private readonly HttpClient _httpClient;
        private string _baseApiUrl = "";

        /// <summary>
        /// Città per cui configurare le notifiche.
        /// </summary>
        public City City
        {
            get => _city ?? throw new InvalidOperationException("City cannot be null");
            set => SetProperty(ref _city, value);
        }

        /// <summary>
        /// Soglia massima di temperatura impostata dall’utente.
        /// </summary>
        public double HighTemperatureThreshold
        {
            get => _highTemperatureThreshold;
            set => SetProperty(ref _highTemperatureThreshold, value);
        }

        /// <summary>
        /// Soglia minima di temperatura impostata dall’utente.
        /// </summary>
        public double LowTemperatureThreshold
        {
            get => _lowTemperatureThreshold;
            set => SetProperty(ref _lowTemperatureThreshold, value);
        }

        /// <summary>
        /// Indica se le notifiche sono abilitate per la città corrente.
        /// </summary>
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

        /// <summary>
        /// Costruttore della ViewModel.
        /// </summary>
        /// <param name="city">Città per la quale gestire le notifiche.</param>
        /// <param name="httpClient">Istanza HttpClient per le richieste API.</param>
        public NotificationSettingsViewModel(City city, HttpClient httpClient)
        {
            City = city ?? throw new ArgumentNullException(nameof(city));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        /// <summary>
        /// Metodo asincrono di inizializzazione. Deve essere chiamato esternamente dopo la creazione della ViewModel.
        /// </summary>
        public async Task InitializeAsync()
        {
            await ConfigService.Instance.InitializeAsync();
            _baseApiUrl = ConfigService.Instance.GetBaseApiUrl() ?? "";

            if (string.IsNullOrEmpty(_baseApiUrl))
            {
                Console.WriteLine("⚠️ Base URL non trovato o vuoto.");
                return;
            }

            await LoadSettingsAsync();
        }

        /// <summary>
        /// Genera una chiave univoca per le preferenze di notifica basata su token e località.
        /// </summary>
        private string GeneratePreferenceKey(string token, string location)
        {
            return $"notifications_enabled_{token}_{location}";
        }

        /// <summary>
        /// Carica le impostazioni salvate dal server per la città corrente.
        /// </summary>
        private async Task LoadSettingsAsync()
        {
            try
            {
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
                Console.WriteLine("Errore LoadSettingsAsync(): " + ex.Message);
            }
        }

        /// <summary>
        /// Salva la preferenza di notifica localmente per il dispositivo corrente.
        /// </summary>
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

        /// <summary>
        /// Salva o aggiorna le impostazioni dell’utente lato server.
        /// </summary>
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
                {
                    _baseApiUrl = ConfigService.Instance.GetBaseApiUrl() ?? "";
                    if (string.IsNullOrEmpty(_baseApiUrl))
                    {
                        Console.WriteLine("⚠️ Base URL mancante, impossibile salvare le impostazioni.");
                        return;
                    }
                }

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
