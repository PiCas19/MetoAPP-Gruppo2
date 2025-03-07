namespace MeteoApp.Models {
    public class WeatherData {
        public WeatherLocation Location { get; set; }
        public WeatherInfo Weather { get; set; }
        public TemperatureInfo Temperature { get; set; }
        public WindInfo Wind { get; set; }
        public CloudInfo Cloud { get; set; }
        public RainInfo Rain { get; set; }
        public SnowInfo Snow { get; set; }
        public long Sunrise { get; set; }
        public long Sunset { get; set; }
        public int Timezone { get; set; } 
    }
}