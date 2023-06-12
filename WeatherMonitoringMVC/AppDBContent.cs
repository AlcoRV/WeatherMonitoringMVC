using WeatherMonitoringMVC.Models;
using Microsoft.EntityFrameworkCore;

namespace WeatherMonitoringMVC
{
    public class AppDBContent: DbContext
    {
        public AppDBContent(DbContextOptions<AppDBContent> options) : base(options) { }

        public DbSet<WeatherDescription> WeatherDescriptions { get; set; }
    }
}
