using System.ComponentModel.DataAnnotations;

namespace WeatherMonitoringMVC.Models
{
    public class WeatherDescription
    {
        [Key]
        public DateTime Date { get; set; }
        public float T { get; set; }
        public byte Humidity { get; set; }
        public float Td { get; set; }
        public ushort Pressure { get; set; }
        public string WindDirection { get; set; }
        public byte WindSpeed { get; set; }
        public byte? CloudCover { get; set; }
        public ushort H { get; set; }
        public byte? VV { get; set; }
        public string WeatherPhenomenon { get; set; }
    }
}
