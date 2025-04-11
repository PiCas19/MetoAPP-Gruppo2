using FirebaseAdmin.Messaging;
using Google.Cloud.Firestore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BackendProject
{
    public class TemperatureMonitoringService : BackgroundService
    {
        private readonly ILogger<TemperatureMonitoringService> _logger;
        private readonly FirestoreDb _firestoreDb;

        public TemperatureMonitoringService(ILogger<TemperatureMonitoringService> logger, FirestoreDb firestoreDb)
        {
            _logger = logger;
            _firestoreDb = firestoreDb;
        }

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

        private Task<double> GetTemperatureForLocationAsync(string location)
        {
            return Task.FromResult(32.0);
        }

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
