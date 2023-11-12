using WeatherMonitoringMVC.Interfaces;
using WeatherMonitoringMVC.Models;
using WeatherMonitoringMVC.Tools;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using X.PagedList;

namespace WeatherMonitoringMVC.Controllers
{
    public class WeatherDescriptionViewModel
    {
        public IPagedList<WeatherDescription> WeatherDescriptions { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }
    }

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IAllWeatherDescription _weatherDescriptions;

        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment webHostEnvironment, IAllWeatherDescription weatherDescriptions)
        {
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
            _weatherDescriptions = weatherDescriptions;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Downloading()
        {
            return View();
        }

        public IActionResult Viewing(int? month, int? year, int? page)
        {
           var weatherDescriptions = _weatherDescriptions.WeatherDescriptions;
            if (month != null && month > 0 && month <= 12) { weatherDescriptions = weatherDescriptions.Where(w => w.Date.Month == month); }
            if (year != null) { weatherDescriptions = weatherDescriptions.Where(w => w.Date.Year == year); }
            
            return View(new WeatherDescriptionViewModel()
            {
                WeatherDescriptions = weatherDescriptions.ToPagedList(page ?? 1, 10),
                Year = year,
                Month = month
            });
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFileCollection files)
        {
            if (files != null && files.Count > 0)
            {
                string tempFilesFolder = Path.Combine(_webHostEnvironment.WebRootPath, "tempFiles");

                foreach(var file in files)
                {
                    string filePath = Path.Combine(tempFilesFolder, file.FileName);
                    using var stream = new FileStream(filePath, FileMode.Create);
                    await file.CopyToAsync(stream);
                }
                // Можно выполнить дополнительные действия с загруженным файлом здесь

                FileProcessor.TryTranslateAllXlsToDB(tempFilesFolder, _weatherDescriptions);
                FileProcessor.ClearDirectory(tempFilesFolder);
                return RedirectToAction("Viewing", "Home"); 
            }

            return View("Downloading");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}