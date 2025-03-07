namespace MeteoApp.Models {
    public class TemperatureInfo {
        public double Current { get; set; }
        public double FeelsLike { get; set; }
        public double Min { get; set; }
        public double Max { get; set; }
        public int Pressure { get; set; }
        public int Humidity { get; set; }
    }
}