using Microsoft.AspNetCore.Mvc;
using Smart_Agriculture_System.Data;
using Smart_Agriculture_System.Models;
using System.Net.Http;
using System.Text.Json;

namespace Smart_Agriculture_System.BackgroundServices
{
    public class SensorDataJob : ISensorDataJob
    {
        private readonly string _dataPath;
        private readonly MongoDBContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly IHttpClientFactory _httpClientFactory;



        public SensorDataJob(IConfiguration configuration, MongoDBContext context,
            IWebHostEnvironment environment, IHttpClientFactory httpClientFactory)
        {
            _dataPath = configuration["Data:DataPath"]!;
            _context = context;
            _environment = environment;
            _httpClientFactory = httpClientFactory;
        }

        public async Task LoadSensorDataAsync()
        {
            var weatherData = await GetEnvironmentWeather();
            await WriteSensorDataToFileAsync(double.Parse(weatherData.Temperature), double.Parse(weatherData.Humidity));
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


        private async Task<CurrentWeather> GetCurrentWeather(double latitude = 31.0539, double longitude = 31.3779)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                var apiUrl = @$"https://api.open-meteo.com/v1/forecast?latitude={latitude}&longitude={longitude}&current=temperature_2m,relative_humidity_2m&timezone=Africa%2FCairo&forecast_days=1";
                var response = await httpClient.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode();

                var jsonString = await response.Content.ReadAsStringAsync();
                var weatherData = JsonSerializer.Deserialize<WeatherApiResponse>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return weatherData.Current;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Error fetching weather data: {ex.Message}");
            }
        }

        private async Task WriteSensorDataToFileAsync(double temperature, double humidity)
        {
            var jsonPath = Path.Combine(_environment.WebRootPath, "Data", "data.json");

            try
            {
                var sensorData = new
                {
                    temperature,
                    humidity
                };

                var json = JsonSerializer.Serialize(sensorData, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                await File.WriteAllTextAsync(jsonPath, json);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to write to JSON file: {ex.Message}", ex);
            }
        }

        private async Task<EnvironmentWeather> GetEnvironmentWeather()
        {
            var httpClient = _httpClientFactory.CreateClient();
            var apiUrl = "https://web-production-856c2.up.railway.app/get_last_environment";
            var response = await httpClient.GetAsync(apiUrl);
            response.EnsureSuccessStatusCode();
            var jsonString = await response.Content.ReadAsStringAsync();
            var weatherData = JsonSerializer.Deserialize<EnvironmentWeather>(jsonString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                
            });
            return weatherData;
        }
    }
}
