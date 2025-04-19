using MeteoAPP.Models;

namespace MeteoAPP.Services {
    public interface IParameterService
    {
       WeatherData GetData();
       void SetData(WeatherData newData);
    }

    public class ParameterService : IParameterService
    {
        private WeatherData _data = new WeatherData();

        public WeatherData GetData()
        {
            return _data;
        }

        public void SetData(WeatherData newData)
        {
            _data = newData;
        }
    }
}
