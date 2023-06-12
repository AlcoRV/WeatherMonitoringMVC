using WeatherMonitoringMVC.Models;

namespace WeatherMonitoringMVC.Interfaces
{
    public interface IAllWeatherDescription
    {
        IEnumerable<WeatherDescription> WeatherDescriptions { get; }
        WeatherDescription GetItem(Func<WeatherDescription, bool> predicate);
        void Add(WeatherDescription item);
        void Add(IEnumerable<WeatherDescription> weatherDescriptions);
    }
}
