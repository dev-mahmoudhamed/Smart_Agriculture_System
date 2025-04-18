using Smart_Agriculture_System.Data;
using Smart_Agriculture_System.Models;
using System.Text.Json;

namespace Smart_Agriculture_System.BackgroundServices
{
    public class SencorDataJob : ISencorDataJob
    {
        private readonly string _dataPath;
        private readonly MongoDBContext _context;

        public SencorDataJob(IConfiguration configuration, MongoDBContext context)
        {
            _dataPath = configuration["Data:DataPath"]!;
            _context = context;
        }

        public async Task ReadSencorDataAsync()
        {
            var data = await ReadTempratureAndHumidity();
            var image = await ReadImage();

            var reading = new Reading
            {
                Temperature = data.Temperature,
                Humidity = data.Humidity,
                ImageAsBase64 = image,
                Time = data.Time
            };

            await _context.Readings.InsertOneAsync(reading);
        }

        private async Task<Reading> ReadTempratureAndHumidity()
        {
            string jsonPath = Path.Combine(_dataPath, "data.json");
            try
            {
                if (!File.Exists(jsonPath))
                    throw new FileNotFoundException($"JSON file not found at {jsonPath}");

                string json = await File.ReadAllTextAsync(jsonPath);

                var data = JsonSerializer.Deserialize<Reading>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return data;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        private async Task<string> ReadImage()
        {
            string imgPath = Path.Combine(_dataPath, "image.jpg");
            try
            {
                if (!File.Exists(imgPath))
                    throw new FileNotFoundException($"JSON file not found at {imgPath}");

                var imageBytes = await File.ReadAllBytesAsync(imgPath);
                string base64String = Convert.ToBase64String(imageBytes);
                return base64String;

            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
