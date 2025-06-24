using Microsoft.AspNetCore.Mvc;
using Smart_Agriculture_System.Models;
using Smart_Agriculture_System.Services;
using System;
using System.Diagnostics;
using System.Text.Json;

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
        public async Task<List<PlantInfo>> Predict()
        {
            var data = await _sensorDataServices.GetAllSensorDataAsync();
            var input = new PredictInput
            {
                Temperature = data.Temperature,
                Humidity = data.Humidity
            };
            var predictionResult = await PredictFromApi(input);
            return predictionResult;
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

     
        [HttpPost("detectDiseases")]
        public async Task<string> DetectDiseases(IFormFile img)
        {
            string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + Path.GetExtension(img.FileName));
            using (var stream = new FileStream(tempPath, FileMode.Create))
            {
                await img.CopyToAsync(stream);
            }
            string result = await ProcessImg(tempPath);
            return result;
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
        private async Task<string> ProcessImg(string tempPath)
        {
            string output = "";
            var psi = new ProcessStartInfo
            {
                FileName = "python", // or full path to python.exe
                Arguments = $"\"D:\\Faculty\\Grad_Project\\capture_photo.py\" \"{tempPath}\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = Process.Start(psi))
            {
                output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
            }
            return output;
        }
        private async Task<byte[]> ConvertToByteArrayAsync(IFormFile formFile)
        {
            var xxx = GetAdvice();
            if (formFile == null || formFile.Length == 0)
                return null;

            using (var memoryStream = new MemoryStream())
            {
                await formFile.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }
        // Capture Photo from camera sensor << It can be deleted >>
        [HttpGet("getSensorImage")]
        private IActionResult GetSensorImageAsync()
        {
            var psi = new ProcessStartInfo
            {
                FileName = "python",
                Arguments = "D:\\Faculty\\Grad_Project\\capture_photo.py",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = Process.Start(psi))
            {
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                if (!output.Contains("OK"))
                    return StatusCode(500, "Failed to capture photo");
            }

            var imagePath = "D:\\Faculty\\Grad_Project\\latest.jpg";
            if (!System.IO.File.Exists(imagePath))
                return StatusCode(500, "Image file not found");

            var imageBytes = System.IO.File.ReadAllBytes(imagePath);
            return File(imageBytes, "image/jpeg");
        }

    }
}
