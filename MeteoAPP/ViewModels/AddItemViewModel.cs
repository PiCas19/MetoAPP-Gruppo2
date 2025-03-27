using System.Text.Json;
using MeteoAPP.Models;

namespace MeteoAPP.ViewModels
{
    public class AddItemViewModel : BaseViewModel
    {
        private string cityName = string.Empty;
        private string countryName = string.Empty;
        private double latitude;
        private double longitude;

        public string CityName
        {
            get => cityName;
            set
            {
                cityName = value;
                OnPropertyChanged();
            }
        }

        public string CountryName
        {
            get => countryName;
            set
            {
                countryName = value;
                OnPropertyChanged();
            }
        }

        public double Latitude
        {
            get => latitude;
            set
            {
                latitude = value;
                OnPropertyChanged();
            }
        }

        public double Longitude
        {
            get => longitude;
            set
            {
                longitude = value;
                OnPropertyChanged();
            }
        }

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
                Console.WriteLine($"Errore durante il recupero della posizione: {ex.Message}");
            }

            return "Posizione non trovata";
        }

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
                    Console.WriteLine($"Risposta API per '{city}': {json}");
                    var data = JsonSerializer.Deserialize<List<JsonElement>>(json);

                    if (data?.Count > 0)
                    {
                        var firstResult = data[0];
                        var lat = firstResult.GetProperty("lat").GetString();
                        var lon = firstResult.GetProperty("lon").GetString();

                        string cityName = "Sconosciuto";
                        string countryName = "Sconosciuto";

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
                        Console.WriteLine($"Nessun risultato trovato per '{city}'.");
                    }
                }
                else
                {
                    Console.WriteLine($"Errore API: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore durante la ricerca della città: {ex.Message}");
            }

            return null;
        }
    }
}