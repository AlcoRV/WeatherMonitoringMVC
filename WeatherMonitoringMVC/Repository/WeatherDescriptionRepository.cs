using WeatherMonitoringMVC.Interfaces;
using WeatherMonitoringMVC.Models;
using WeatherMonitoringMVC.Tools;

namespace WeatherMonitoringMVC.Repository
{
    public class WeatherDescriptionRepository : IAllWeatherDescription
    {
        private readonly AppDBContent _db;

        public WeatherDescriptionRepository(AppDBContent db)
        {
            this._db = db;
        }

        public WeatherDescription GetItem(Func<WeatherDescription, bool> predicate) => _db.WeatherDescriptions.FirstOrDefault(predicate);

        public IEnumerable<WeatherDescription> WeatherDescriptions
        {
            get
            {
                if (!_db.WeatherDescriptions.ToList().Any())
                {
                    string pathDefaultData = ".\\wwwroot\\defaultData\\moskva_2010.xlsx";
                    FileProcessor.TryTranslateXlsToDB(pathDefaultData, this);
                }
                return _db.WeatherDescriptions;
            }
        }

        public void Add(WeatherDescription weatherDescription)
        {
            _db.WeatherDescriptions.Add(weatherDescription);
            _db.SaveChanges();
        }

        public void Add(IEnumerable<WeatherDescription> weatherDescriptions)
        {
            _db.WeatherDescriptions.AddRange(weatherDescriptions);
            _db.SaveChanges();
        }
    }
}
