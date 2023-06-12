using WeatherMonitoringMVC.Interfaces;
using WeatherMonitoringMVC.Models;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace WeatherMonitoringMVC.Tools
{
    public static class FileProcessor
    {
        public static void TranslateXlsToDB(string filePath, IAllWeatherDescription weatherDescriptions)
        {
            IWorkbook workbook = GetWorkbook(filePath);

            if(workbook == null) { return; }

            var list = new LinkedList<WeatherDescription>();

            foreach (var sheet in workbook)
            {
                if(sheet == null) { return; }

                var lastRowNum = sheet.LastRowNum;
                for (int i = 5; i <= lastRowNum; i++)
                {
                    var row = sheet.GetRow(i);
                    if(row == null) { return; }

                    var weatherDescription = CreateWeatherDescription(row);
                    list.AddLast(weatherDescription);
                }
            }
            weatherDescriptions.Add(list);
        }

        private static WeatherDescription CreateWeatherDescription(IRow row)
        {
            var dateFirst = DateTime.Parse(row.GetCell(0).SetTypeToString().StringOrEmptyCellValue());
            var dateSecond = DateTime.Parse(row.GetCell(1).SetTypeToString().StringOrEmptyCellValue());
            var t = (float?)row.GetCell(2).SetTypeToString().ShirtNACellValue() ?? 0;
            var humidity = (byte?)row.GetCell(3).SetTypeToString().ShirtNACellValue() ?? 0;
            var td = (float?)row.GetCell(4).SetTypeToString().ShirtNACellValue() ?? 0;
            var pressure = (ushort?)row.GetCell(5).SetTypeToString().ShirtNACellValue() ?? 0;
            var windDirection = row.GetCell(6).SetTypeToString().StringOrEmptyCellValue();
            var windSpeed = (byte?)row.GetCell(7).SetTypeToString().ShirtNACellValue() ?? 0;
            var cloudCover = (byte?)row.GetCell(8).SetTypeToString().ShirtNACellValue();
            var h = (ushort?)row.GetCell(9).SetTypeToString().ShirtNACellValue() ?? 0;
            var vv = (byte?)row.GetCell(10).SetTypeToString().ShirtNACellValue();
            var weatherPhenomenon = row.GetCell(11).SetTypeToString().StringOrEmptyCellValue();
            
            var date = dateFirst.Add(dateSecond.TimeOfDay);
            
            return new WeatherDescription()
            {
                Date = date,
                T = t,
                Humidity = humidity,
                Td = td,
                Pressure = pressure,
                WindDirection = windDirection,
                WindSpeed = windSpeed,
                CloudCover = cloudCover,
                H = h,
                VV = vv,
                WeatherPhenomenon = weatherPhenomenon
            };
        }

        private static ICell SetTypeToString(this ICell cell)
        {
            if(cell != null) { cell.SetCellType(CellType.String); }
            return cell;
        }

        private static short? ShirtNACellValue(this ICell cell)
        {
            try
            {
                if(short.TryParse(cell.StringCellValue, out short value))
                {
                    return value;
                }
                return null;
            }
            catch (Exception)
            {

                return null;
            }
        }

        private static string StringOrEmptyCellValue(this ICell cell)
        {
            if(cell == null) { return string.Empty; }
            return cell.StringCellValue;
        }

        private static IWorkbook GetWorkbook(string filePath)
        {
            IWorkbook workbook = null;
            var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            if (filePath.IndexOf(".xlsx") > 0)
            {
                workbook = new XSSFWorkbook(fs);
            }
            else if (filePath.IndexOf(".xls") > 0)
            {
                workbook = new HSSFWorkbook(fs);
            }

            return workbook;
        }

        public static bool TryTranslateXlsToDB(string filePath, IAllWeatherDescription weatherDescriptions)
        {
            try
            {
                TranslateXlsToDB(filePath, weatherDescriptions);
                return true;
            }
            catch (Exception e)
            {
                var eMessage = e.Message;
                //Any additional actions
            }
            return false;
        }

        public static void TranslateAllXlsToDB(string directoryPath, IAllWeatherDescription weatherDescriptions)
        {
            var filePaths = Directory.GetFiles(directoryPath);
            if(filePaths != null)
            {
                foreach (var filePath in filePaths)
                {
                    TranslateXlsToDB(filePath, weatherDescriptions);
                }
            }
        }

        public static bool TryTranslateAllXlsToDB(string directoryPath, IAllWeatherDescription weatherDescriptions)
        {
            var filePaths = Directory.GetFiles(directoryPath);
            if (filePaths != null)
            {
                foreach (var filePath in filePaths)
                {
                    if (!TryTranslateXlsToDB(filePath, weatherDescriptions))
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        public static void ClearDirectory(string directoryPath)
        {
            var filePaths = Directory.GetFiles(directoryPath);
            if (filePaths != null)
            {
                foreach (var filePath in filePaths)
                {
                    File.Delete(filePath);
                }
            }
        }
    }
}
