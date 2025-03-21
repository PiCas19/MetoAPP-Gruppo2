using SQLite;
using Newtonsoft.Json;
namespace MeteoApp.Models 
{
    public class WeatherData 
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string LocationJson { get; set; }
        public string WeatherJson { get; set; }
        public string TemperatureJson { get; set; }
        public string WindJson { get; set; }
        public string CloudJson { get; set; }
        public string RainJson { get; set; }
        public string SnowJson { get; set; }
        public long Sunrise { get; set; }
        public long Sunset { get; set; }
        public int Timezone { get; set; }
        
        [Ignore] 
        public WeatherLocation Location
        {
            get => !string.IsNullOrEmpty(LocationJson) ? 
                JsonConvert.DeserializeObject<WeatherLocation>(LocationJson) : 
                new WeatherLocation();
            set => LocationJson = JsonConvert.SerializeObject(value);
        }
        
        [Ignore] 
        public WeatherInfo Weather
        {
            get => !string.IsNullOrEmpty(WeatherJson) ? 
                JsonConvert.DeserializeObject<WeatherInfo>(WeatherJson) : 
                new WeatherInfo();
            set => WeatherJson = JsonConvert.SerializeObject(value);
        }
        
        [Ignore] 
        public TemperatureInfo Temperature
        {
            get => !string.IsNullOrEmpty(TemperatureJson) ? 
                JsonConvert.DeserializeObject<TemperatureInfo>(TemperatureJson) : 
                new TemperatureInfo();
            set => TemperatureJson = JsonConvert.SerializeObject(value);
        }
        
        [Ignore] 
        public WindInfo Wind
        {
            get => !string.IsNullOrEmpty(WindJson) ? 
                JsonConvert.DeserializeObject<WindInfo>(WindJson) : 
                new WindInfo();
            set => WindJson = JsonConvert.SerializeObject(value);
        }
        
        [Ignore] 
        public CloudInfo Cloud
        {
            get => !string.IsNullOrEmpty(CloudJson) ? 
                JsonConvert.DeserializeObject<CloudInfo>(CloudJson) : 
                new CloudInfo();
            set => CloudJson = JsonConvert.SerializeObject(value);
        }
        
        [Ignore] 
        public RainInfo Rain
        {
            get => !string.IsNullOrEmpty(RainJson) ? 
                JsonConvert.DeserializeObject<RainInfo>(RainJson) : 
                new RainInfo();
            set => RainJson = JsonConvert.SerializeObject(value);
        }
        
        [Ignore] 
        public SnowInfo Snow
        {
            get => !string.IsNullOrEmpty(SnowJson) ? 
                JsonConvert.DeserializeObject<SnowInfo>(SnowJson) : 
                new SnowInfo();
            set => SnowJson = JsonConvert.SerializeObject(value);
        }
        
        public WeatherData()
        {
            // Inizializza stringhe vuote per evitare errori di deserializzazione
            LocationJson = "{}";
            WeatherJson = "{}";
            TemperatureJson = "{}";
            WindJson = "{}";
            CloudJson = "{}";
            RainJson = "{}";
            SnowJson = "{}";
        }
    }
}