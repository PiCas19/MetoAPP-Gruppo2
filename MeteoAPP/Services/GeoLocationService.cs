using MeteoAPP.Models;

namespace MeteoAPP.Services
{

    /// <summary>
    /// Servizio per ottenere la posizione geografica corrente del dispositivo.
    /// Richiede i permessi di localizzazione e restituisce latitudine e longitudine.
    /// </summary>
    public class GeoLocationService
    {
        /// <summary>
        /// Ottiene la posizione attuale del dispositivo.
        /// </summary>
        /// <returns>
        /// Un oggetto <see cref="GeoLocationResult"/> che indica il successo dell'operazione e, se disponibile,
        /// le coordinate geografiche (latitudine e longitudine).
        /// </returns>
        public async Task<GeoLocationResult> GetCurrentLocationAsync()
        {
            try
            {
                var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
                if (status != PermissionStatus.Granted)
                {
                    status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                    if (status != PermissionStatus.Granted)
                    {
                        return new GeoLocationResult 
                        { 
                            Success = false, 
                            ErrorMessage = "Location permission denied" 
                        };
                    }
                }

                var location = await Geolocation.GetLocationAsync(new GeolocationRequest(GeolocationAccuracy.Best));
                if (location != null)
                {
                    return new GeoLocationResult
                    {
                        Success = true,
                        Latitude = location.Latitude,
                        Longitude = location.Longitude
                    };
                }
                else
                {
                    return new GeoLocationResult 
                    { 
                        Success = false, 
                        ErrorMessage = "Unable to retrieve location" 
                    };
                }
            }
            catch (Exception ex)
            {
                return new GeoLocationResult 
                { 
                    Success = false, 
                    ErrorMessage = ex.Message 
                };
            }
        }
    }
}