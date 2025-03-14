namespace MeteoApp.Models {
    public class WeatherInfo {
        public int Id { get; set; }
        public string Main { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        
        public WeatherInfo()
        {
            Main = string.Empty;
            Description = string.Empty;
            Icon = string.Empty;
        }
    }
}