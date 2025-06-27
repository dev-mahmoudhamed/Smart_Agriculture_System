using Microsoft.AspNetCore.Mvc;
using Smart_Agriculture_System.Models;
using Smart_Agriculture_System.Services;
using System.Diagnostics;
using System.Text.Json;
using System.Net.Http.Headers;

namespace Smart_Agriculture_System.Controllers
{
    [ApiController]
    [Route("api/readings")]
    public class ReadingsController : ControllerBase
    {
        private readonly ISensorDataServices _sensorDataServices;

        private readonly IGeminiServices _geminiServices;

        public ReadingsController(IGeminiServices geminiServices, ISensorDataServices sensorDataServices)
        {
            _geminiServices = geminiServices;
            _sensorDataServices = sensorDataServices;
        }

        // Read last data recorded by sensors for humidity and temperature
        [HttpGet("getSensorData")]
        public async Task<SensorReading> GetSensorDataAsync()
        {
            var readings = await _sensorDataServices.GetAllSensorDataAsync();
            return readings;
        }

        [HttpPost("predict")]
        public async Task<PlantInfo> Predict()
        {
            var data = await _sensorDataServices.GetAllSensorDataAsync();
            var input = new PredictInput
            {
                Temperature = data.Temperature,
                Humidity = data.Humidity
            };
            var predictionResult = await PredictFromApi(input);
            return predictionResult.First();
        }

        [HttpGet("getAdvice")]
        public async Task<FlutterResponceObject> GetAdvice()
        {
            var response = await _geminiServices.GetAdvice();
            return new FlutterResponceObject
            {
                Result = response,
            };
        }

        [HttpPost("detect")]
        public async Task<string> DetectDiseases(IFormFile img)
        {
            string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + Path.GetExtension(img.FileName));
            using (var stream = new FileStream(tempPath, FileMode.Create))
            {
                await img.CopyToAsync(stream);
            }
            var apiResult = await ProcessImg(tempPath);


            var formattedClass = apiResult.Class.Replace("___", " - ").Replace("_", " ");

            // Convert confidence to percentage with 1 decimal
            var confidencePercent = (apiResult.Confidence * 100).ToString("F1") + "%";

            // Build result string
            string resultMessage =  $@"Detected Disease: {formattedClass} Confidence: {confidencePercent} 
Treatment Recommendation: {apiResult.Treatment}";

            Console.WriteLine(resultMessage);
        
            return resultMessage;
        }


        private async Task<List<PlantInfo>> PredictFromApi(PredictInput input)
        {
            string url = $"https://web-production-856c2.up.railway.app/predict_plant?Temperature={input.Temperature}&Humidity={input.Humidity}";
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    var response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    var json = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    return JsonSerializer.Deserialize<List<PlantInfo>>(json, options);

                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine($"Request error: {e.Message}");
                    throw;
                }
            }
        }
        private async Task<HealthPredictionResponse> ProcessImg(string imagePath)
        {
            var url = "https://web-production-5ff01.up.railway.app/predict_health";

            using var httpClient = new HttpClient();

            byte[] imageData = await System.IO.File.ReadAllBytesAsync(imagePath);

            using var content = new ByteArrayContent(imageData);
            content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");

            var response = await httpClient.PostAsync(url, content);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var result = JsonSerializer.Deserialize<HealthPredictionResponse>(responseContent, options);

            return result;
        }
    }
}
