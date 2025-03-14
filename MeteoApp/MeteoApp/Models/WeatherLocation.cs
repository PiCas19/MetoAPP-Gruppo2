namespace MeteoApp.Models {
    public class WeatherLocation {
        public string Name { get; set; }
        public double Lon { get; set; }
        public double Lat { get; set; }

        public WeatherLocation()
        {
            Name = string.Empty;
        }

    }

}