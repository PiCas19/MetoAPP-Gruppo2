using System.Text.Json;
using MeteoAPP.Models;

namespace MeteoAPP.ViewModels
{
    /// <summary>
    /// ViewModel per l'aggiunta di una nuova città tramite geolocalizzazione o ricerca per nome.
    /// Include logica per ottenere nome città da coordinate e viceversa.
    /// </summary>
    public class AddItemViewModel : BaseViewModel
    {
        private string cityName = string.Empty;
        private string countryName = string.Empty;
        private double latitude;
        private double longitude;
        /// <summary>
        /// Nome della città selezionata.
        /// </summary>
        public string CityName
        {
            get => cityName;
            set
            {
                cityName = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// Nome del paese associato alla città.
        /// </summary>
        public string CountryName
        {
            get => countryName;
            set
            {
                countryName = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// Latitudine della città.
        /// </summary>
        public double Latitude
        {
            get => latitude;
            set
            {
                latitude = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// Longitudine della città.
        /// </summary>
        public double Longitude
        {
            get => longitude;
            set
            {
                longitude = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Ottiene il nome della località (città e paese) da coordinate geografiche utilizzando Nominatim.
        /// </summary>
        /// <param name="latitude">Latitudine della posizione.</param>
        /// <param name="longitude">Longitudine della posizione.</param>
        /// <returns>Il nome della città se trovato, altrimenti "Position not found".</returns>
        public async Task<string> GetLocationNameAsync(double latitude, double longitude)
        {
            string url = $"https://nominatim.openstreetmap.org/reverse?format=json&lat={latitude}&lon={longitude}";

            try
            {
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Add("User-Agent", "MeteoApp/1.0");
                var response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var data = JsonSerializer.Deserialize<JsonElement>(json);

                    if (data.TryGetProperty("address", out JsonElement address))
                    {
                        string city = address.TryGetProperty("city", out JsonElement cityElement) ? cityElement.GetString() ?? "Sconosciuto" :
                                      address.TryGetProperty("town", out JsonElement townElement) ? townElement.GetString() ?? "Sconosciuto" :
                                      address.TryGetProperty("village", out JsonElement villageElement) ? villageElement.GetString() ?? "Sconosciuto" :
                                      address.TryGetProperty("county", out JsonElement countyElement) ? countyElement.GetString() ?? "Sconosciuto" : "Sconosciuto";

                        string country = address.TryGetProperty("country", out JsonElement countryElement) ? countryElement.GetString() ?? "Sconosciuto" : "Sconosciuto";

                        CityName = city;
                        CountryName = country;

                        return city;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during position recovery: {ex.Message}");
            }

            return "Position not found";
        }

        /// <summary>
        /// Ottiene le coordinate geografiche da un nome città usando Nominatim API.
        /// </summary>
        /// <param name="city">Nome della città da cercare.</param>
        /// <returns>
        /// Oggetto <see cref="GeoLocation"/> contenente nome, latitudine e longitudine, oppure null se la città non è trovata.
        /// </returns>
        public async Task<GeoLocation?> GetCoordinatesFromCityAsync(string city)
        {
            string encodedCity = Uri.EscapeDataString(city.Replace(" ", "+"));
            string url = $"https://nominatim.openstreetmap.org/search?format=json&q={encodedCity}";

            try
            {
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Add("User-Agent", "MeteoApp/1.0");
                var response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var data = JsonSerializer.Deserialize<List<JsonElement>>(json);

                    if (data?.Count > 0)
                    {
                        var firstResult = data[0];
                        var lat = firstResult.GetProperty("lat").GetString();
                        var lon = firstResult.GetProperty("lon").GetString();

                        string cityName = "Unknown";
                        string countryName = "Unknown";

                        if (firstResult.TryGetProperty("display_name", out JsonElement displayName))
                        {
                            var parts = displayName.GetString()?.Split(',');
                            if (parts?.Length > 0)
                            {
                                cityName = parts[0].Trim();
                            }
                            if (parts?.Length > 1)
                            {
                                countryName = parts[^1].Trim();
                            }
                        }

                        CityName = cityName;
                        CountryName = countryName;
                        Latitude = double.Parse(lat ?? "0");
                        Longitude = double.Parse(lon ?? "0");

                        return new GeoLocation
                        {
                            Name = cityName,
                            Latitude = Latitude,
                            Longitude = Longitude
                        };
                    }
                    else
                    {
                        Console.WriteLine($"No results found for '{city}'.");
                    }
                }
                else
                {
                    Console.WriteLine($"API error: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during city search: {ex.Message}");
            }

            return null;
        }
    }
}