namespace MeteoAPP.Models
{
    public class WeatherData
    {
        public string Location { get; set; } = "N/A";
        public string Description { get; set; } = "N/A";
        public string IconCode { get; set; } = "N/A";
        public double Temperature { get; set; }
        public double TemperatureMin { get; set; }
        public double TemperatureMax { get; set; }
    }
}