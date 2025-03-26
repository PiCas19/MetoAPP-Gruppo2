using SQLite;

namespace MeteoAPP.Models
{
    public class City
    {
        [PrimaryKey, AutoIncrement]
        public long Id { get; set; }

        public double Longitude { get; set; }

        public double Latitude { get; set; }

        public string? Name { get; set; }

        public string? Country { get; set; }
    }
}
