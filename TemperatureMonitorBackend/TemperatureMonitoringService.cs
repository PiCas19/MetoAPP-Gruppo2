using FirebaseAdmin.Messaging;
using Google.Cloud.Firestore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BackendProject
{

    /// <summary>
    /// Servizio in background che monitora periodicamente le condizioni meteo
    /// e invia notifiche push agli utenti quando le soglie configurate vengono superate.
    /// </summary>
    public class TemperatureMonitoringService : BackgroundService
    {
        private readonly ILogger<TemperatureMonitoringService> _logger;
        private readonly FirestoreDb _firestoreDb;

        /// <summary>
        /// Costruttore del servizio di monitoraggio.
        /// </summary>
        /// <param name="logger">Istanza del logger per la registrazione degli eventi.</param>
        /// <param name="firestoreDb">Istanza del database Firestore per accedere ai dati utente.</param>
        public TemperatureMonitoringService(ILogger<TemperatureMonitoringService> logger, FirestoreDb firestoreDb)
        {
            _logger = logger;
            _firestoreDb = firestoreDb;
        }

        /// <summary>
        /// Metodo principale eseguito dal servizio in background.
        /// Controlla le impostazioni utente ogni 5 minuti e invia notifiche se necessario.
        /// </summary>
        /// <param name="stoppingToken">Token di annullamento per la gestione dell'arresto del servizio.</param>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            TimeSpan interval = TimeSpan.FromMinutes(5);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var settingsRef = _firestoreDb.Collection("settings");
                    var query = settingsRef.WhereEqualTo("IsEnabled", true);
                    var snapshot = await query.GetSnapshotAsync();

                    foreach (var doc in snapshot.Documents)
                    {
                        var settings = doc.ConvertTo<UserNotificationSettings>();

                        double currentTemp = await GetTemperatureForLocationAsync(settings.Location);

                        if (currentTemp > settings.TemperatureMax || currentTemp < settings.TemperatureMin)
                        {
                            await SendTemperatureAlertAsync(settings.Token, settings.Location, currentTemp, settings.TemperatureMax, settings.TemperatureMin);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Errore durante il monitoraggio della temperatura.");
                }

                await Task.Delay(interval, stoppingToken);
            }
        }

        /// <summary>
        /// Recupera la temperatura attuale per una determinata località.
        /// In questo esempio è simulata.
        /// </summary>
        /// <param name="location">Nome della località.</param>
        /// <returns>Temperatura simulata in gradi Celsius.</returns>

        private Task<double> GetTemperatureForLocationAsync(string location)
        {
            return Task.FromResult(32.0);
        }

        /// <summary>
        /// Invia una notifica push all'utente tramite Firebase Cloud Messaging
        /// se la temperatura supera le soglie definite.
        /// </summary>
        /// <param name="deviceToken">Token FCM del dispositivo destinatario.</param>
        /// <param name="location">Località monitorata.</param>
        /// <param name="currentTemp">Temperatura attuale rilevata.</param>
        /// <param name="tempMax">Soglia massima configurata dall'utente.</param>
        /// <param name="tempMin">Soglia minima configurata dall'utente.</param>
        private async Task SendTemperatureAlertAsync(string deviceToken, string location, double currentTemp, double tempMax, double tempMin)
        {
            var message = new Message()
            {
                Token = deviceToken,
                Notification = new Notification
                {
                    Title = "Allerta temperatura",
                    Body = $"La temperatura a {location} è di {currentTemp}°C (Soglia: >{tempMax}° o <{tempMin}°)."
                }
            };

            try
            {
                string response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
                _logger.LogInformation($"Notifica inviata con successo: {response}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'invio della notifica.");
            }
        }
    }
}
