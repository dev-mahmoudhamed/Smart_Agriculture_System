using Smart_Agriculture_System.Data;
using Smart_Agriculture_System.Models;
using System.Text.Json;

namespace Smart_Agriculture_System.BackgroundServices
{
    public class SensorDataJob : ISensorDataJob
    {
        private readonly string _dataPath;
        private readonly MongoDBContext _context;
        private readonly IWebHostEnvironment _environment;

        public SensorDataJob(IConfiguration configuration, MongoDBContext context, IWebHostEnvironment environment)
        {
            _dataPath = configuration["Data:DataPath"]!;
            _context = context;
            _environment = environment;
        }

        public async Task LoadSensorDataAsync()
        {
            var jsonPath = Path.Combine(_environment.WebRootPath, "Data", "data.json");
            try
            {
                if (!File.Exists(jsonPath))
                    throw new FileNotFoundException($"JSON file not found at {jsonPath}");

                string json = await File.ReadAllTextAsync(jsonPath);

                var data = JsonSerializer.Deserialize<SensorReading>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                data.Time = DateTime.Now.ToLocalTime();
                await _context.SensorReadings.InsertOneAsync(data);

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task LoadImageDataAsync()
        {
            var imgPath = Path.Combine(_environment.WebRootPath, "Data", "image.jpg");
            try
            {
                if (!File.Exists(imgPath))
                    throw new FileNotFoundException($"JSON file not found at {imgPath}");

                var imageBytes = await File.ReadAllBytesAsync(imgPath);
                string base64String = Convert.ToBase64String(imageBytes);

                var imageData = new ImageReading
                {
                    ImageAsBase64 = base64String,
                    Time = DateTime.Now.ToLocalTime()
                };
                await _context.ImageReadings.InsertOneAsync(imageData);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
