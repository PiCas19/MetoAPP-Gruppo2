using FirebaseAdmin.Messaging;
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

        public TemperatureMonitoringService(ILogger<TemperatureMonitoringService> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Imposta l'intervallo di controllo (ad esempio, ogni 5 minuti)
            TimeSpan interval = TimeSpan.FromMinutes(5);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Recupera le impostazioni degli utenti dal repository in memoria
                    var userSettings = NotificationSettingsRepository.Settings.ToArray();

                    foreach (var settings in userSettings)
                    {
                        // Recupera la temperatura attuale per la località dell'utente (qui simulata)
                        double currentTemp = await GetTemperatureForLocationAsync(settings.Location);

                        // Controlla se la temperatura supera le soglie configurate
                        if (currentTemp > settings.TemperatureMax || currentTemp < settings.TemperatureMin)
                        {
                            // Invia una notifica push
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

        // Metodo simulato per ottenere la temperatura attuale da un'API meteo (da sostituire con una chiamata HTTP reale)
        private Task<double> GetTemperatureForLocationAsync(string location)
        {
            // Per l'esempio, restituisce un valore fisso
            return Task.FromResult(32.0);
        }

        // Metodo per inviare la notifica tramite Firebase Admian SDK
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