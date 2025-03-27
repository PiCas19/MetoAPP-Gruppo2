namespace MeteoAPP.Models
{
    public class GeoLocationResult
    {
        public bool Success { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
    }
}
