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
        public double WindSpeedKmh { get; set; }  
        public double RainChancePercent { get; set; }  
        public double PressureHpa { get; set; } 
        public double MorningTemp { get; set; }
        public double AfternoonTemp { get; set; }
        public double EveningTemp { get; set; }
        public double NightTemp { get; set; }
    }
}