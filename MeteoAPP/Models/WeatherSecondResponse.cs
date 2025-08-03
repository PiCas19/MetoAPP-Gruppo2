using Newtonsoft.Json;
using System.Collections.Generic;

namespace MeteoAPP.Models
{
    public class WeatherSecondResponse
    {
        [JsonProperty("location")]
        public Location Location { get; set; } = null!;

        [JsonProperty("current")]
        public Current Current { get; set; } = null!;

        [JsonProperty("forecast")]
        public Forecast Forecast { get; set; } = null!;
    }

    public class Location
    {
        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;
    }

    public class Current
    {
        [JsonProperty("temp_c")]
        public double TempC { get; set; }

        [JsonProperty("condition")]
        public Condition Condition { get; set; } = null!;

        [JsonProperty("wind_kph")]
        public double WindKph { get; set; }

        [JsonProperty("pressure_mb")]
        public double PressureMb { get; set; }
    }

    public class Forecast
    {
        [JsonProperty("forecastday")]
        public List<ForecastDay> ForecastDays { get; set; } = new();
    }

    public class ForecastDay
    {
        [JsonProperty("date")]
        public string? Date { get; set; }

        [JsonProperty("day")]
        public Day Day { get; set; } = null!;

        [JsonProperty("hour")]
        public List<Hour> Hours { get; set; } = new();
    }

    public class Day
    {
        [JsonProperty("maxtemp_c")]
        public double MaxTempC { get; set; }

        [JsonProperty("mintemp_c")]
        public double MinTempC { get; set; }

        [JsonProperty("condition")]
        public Condition Condition { get; set; } = null!;

        [JsonProperty("daily_chance_of_rain")]
        public double DailyChanceOfRain { get; set; }
    }

    public class Hour
    {
        [JsonProperty("time")]
        public string Time { get; set; } = string.Empty;

        [JsonProperty("temp_c")]
        public double TempC { get; set; }
    }

    public class Condition
    {
        [JsonProperty("text")]
        public string Text { get; set; } = string.Empty;

        [JsonProperty("icon")]
        public string Icon { get; set; } = string.Empty;

        [JsonProperty("code")]
        public int Code { get; set; }
    }
}
