// File: Services/DatabaseSeeder.cs
using MeteoApp.Models;
using MeteoApp.Services;
using System.Diagnostics;

namespace MeteoApp.Services
{
    public static class DatabaseSeeder
    {
        public static async Task SeedDatabaseAsync(WeatherDatabase database)
        {
            try
            {
                // Verifica prima se ci sono già dati nel database
                var existingData = await database.GetWeatherDataAsync();
                if (existingData.Count > 0)
                {
                    Debug.WriteLine("Il database contiene già dei dati, il seeding verrà saltato.");
                    return;
                }

                // Crea alcuni dati di esempio per diverse città
                var cities = new List<WeatherData>
                {
                    CreateRomeWeatherData(),
                    CreateMilanWeatherData(),
                    CreateNaplesWeatherData(),
                    CreateTurinWeatherData(),
                    CreateFlorenceWeatherData()
                };

                // Inserisci i dati nel database
                foreach (var city in cities)
                {
                    await database.SaveWeatherDataAsync(city);
                    Debug.WriteLine($"Aggiunta la città: {city.Location.Name}");
                }

                Debug.WriteLine("Database popolato con successo con dati di esempio.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Errore durante il seeding del database: {ex.Message}");
            }
        }

        private static WeatherData CreateRomeWeatherData()
        {
            return new WeatherData
            {
                Location = new WeatherLocation
                {
                    Name = "Roma",
                    Lat = 41.9028,
                    Lon = 12.4964
                },
                Weather = new WeatherInfo
                {
                    Id = 800,
                    Main = "Clear",
                    Description = "cielo sereno",
                    Icon = "01d"
                },
                Temperature = new TemperatureInfo
                {
                    Current = 25.7,
                    FeelsLike = 26.1,
                    Min = 23.2,
                    Max = 27.8,
                    Pressure = 1012,
                    Humidity = 55
                },
                Wind = new WindInfo
                {
                    Speed = 3.6,
                    Deg = 230,
                    Gust = 5.2
                },
                Cloud = new CloudInfo
                {
                    All = 5
                },
                Rain = new RainInfo(),
                Snow = new SnowInfo(),
                Sunrise = 1647840000, 
                Sunset = 1647883800,  
                Timezone = 3600
            };
        }

        private static WeatherData CreateMilanWeatherData()
        {
            return new WeatherData
            {
                Location = new WeatherLocation
                {
                    Name = "Milano",
                    Lat = 45.4642,
                    Lon = 9.1900
                },
                Weather = new WeatherInfo
                {
                    Id = 801,
                    Main = "Clouds",
                    Description = "nubi sparse",
                    Icon = "02d"
                },
                Temperature = new TemperatureInfo
                {
                    Current = 22.3,
                    FeelsLike = 22.1,
                    Min = 19.8,
                    Max = 24.6,
                    Pressure = 1008,
                    Humidity = 65
                },
                Wind = new WindInfo
                {
                    Speed = 2.1,
                    Deg = 180,
                    Gust = 3.4
                },
                Cloud = new CloudInfo
                {
                    All = 32
                },
                Rain = new RainInfo(),
                Snow = new SnowInfo(),
                Sunrise = 1647839400, 
                Sunset = 1647883200,  
                Timezone = 3600
            };
        }

        private static WeatherData CreateNaplesWeatherData()
        {
            return new WeatherData
            {
                Location = new WeatherLocation
                {
                    Name = "Napoli",
                    Lat = 40.8518,
                    Lon = 14.2681
                },
                Weather = new WeatherInfo
                {
                    Id = 500,
                    Main = "Rain",
                    Description = "pioggia leggera",
                    Icon = "10d"
                },
                Temperature = new TemperatureInfo
                {
                    Current = 24.8,
                    FeelsLike = 25.2,
                    Min = 22.9,
                    Max = 26.3,
                    Pressure = 1010,
                    Humidity = 72
                },
                Wind = new WindInfo
                {
                    Speed = 4.2,
                    Deg = 210,
                    Gust = 6.7
                },
                Cloud = new CloudInfo
                {
                    All = 45
                },
                Rain = new RainInfo
                {
                    OneHour = 0.8,
                },
                Snow = new SnowInfo(),
                Sunrise = 1647840300, 
                Sunset = 1647884100,  
                Timezone = 3600
            };
        }

        private static WeatherData CreateTurinWeatherData()
        {
            return new WeatherData
            {
                Location = new WeatherLocation
                {
                    Name = "Torino",
                    Lat = 45.0703,
                    Lon = 7.6869
                },
                Weather = new WeatherInfo
                {
                    Id = 802,
                    Main = "Clouds",
                    Description = "nubi sparse",
                    Icon = "03d"
                },
                Temperature = new TemperatureInfo
                {
                    Current = 21.5,
                    FeelsLike = 21.2,
                    Min = 18.9,
                    Max = 23.7,
                    Pressure = 1007,
                    Humidity = 68
                },
                Wind = new WindInfo
                {
                    Speed = 1.8,
                    Deg = 170,
                    Gust = 2.9
                },
                Cloud = new CloudInfo
                {
                    All = 38
                },
                Rain = new RainInfo(),
                Snow = new SnowInfo(),
                Sunrise = 1647839100, 
                Sunset = 1647882900, 
                Timezone = 3600
            };
        }

        private static WeatherData CreateFlorenceWeatherData()
        {
            return new WeatherData
            {
                Location = new WeatherLocation
                {
                    Name = "Firenze",
                    Lat = 43.7696,
                    Lon = 11.2558
                },
                Weather = new WeatherInfo
                {
                    Id = 800,
                    Main = "Clear",
                    Description = "cielo sereno",
                    Icon = "01d"
                },
                Temperature = new TemperatureInfo
                {
                    Current = 26.2,
                    FeelsLike = 26.5,
                    Min = 23.8,
                    Max = 28.1,
                    Pressure = 1011,
                    Humidity = 52
                },
                Wind = new WindInfo
                {
                    Speed = 2.8,
                    Deg = 220,
                    Gust = 4.1
                },
                Cloud = new CloudInfo
                {
                    All = 3
                },
                Rain = new RainInfo(),
                Snow = new SnowInfo(),
                Sunrise = 1647839700, 
                Sunset = 1647883500,  
                Timezone = 3600
            };
        }
    }
}