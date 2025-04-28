using Microsoft.AspNetCore.Mvc;
using Smart_Agriculture_System.Models;
using Smart_Agriculture_System.Services;
using System.Buffers.Text;
using System.Diagnostics;
using System.Text;

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
        public async Task<FlutterResponceObject> Predict()
        {
            var data = await _sensorDataServices.GetAllSensorDataAsync();
            var input = new PredictInput
            {
                Temperature = data.Temperature,
                Humidity = data.Humidity
            };
            try
            {
                var predictionResult = await ProcessPredictionFromPython(input);
                return new FlutterResponceObject
                {
                    Result = predictionResult,
                };
            }
            catch (Exception)
            {
                var geminiPredictionString = (await _geminiServices.Predict(input)).ToString();
                var result = string.Concat($"Based on the current temperature :{input.Temperature} and humidity: {input.Humidity}, ", geminiPredictionString);
                return new FlutterResponceObject
                {
                    Result = result,
                };
            }
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
        public async Task<object> DetectDiseases(IFormFile img)
        {
            string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + Path.GetExtension(img.FileName));
            using (var stream = new FileStream(tempPath, FileMode.Create))
            {
                await img.CopyToAsync(stream);
            }

            await ProcessImg(tempPath);
            return null;
        }


        private async Task<string> ProcessPredictionFromPython(PredictInput input)
        {
            string data = $"{input.Temperature},{input.Humidity}";
            var psi = new ProcessStartInfo
            {
                FileName = "python",  // or full path to python.exe
                Arguments = $"\"D:\\Faculty\\Grad_Project\\predict_plant.py\" \"{data}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(psi);
            if (process == null)
                throw new InvalidOperationException("Failed to start Python process.");

            var stdOut = process.StandardOutput.ReadToEndAsync();
            var stdErr = process.StandardError.ReadToEndAsync();
            await Task.WhenAll(stdOut, stdErr).ConfigureAwait(false);
            process.WaitForExit();

            if (!string.IsNullOrWhiteSpace(stdErr.Result))
                throw new Exception($"Python error: {stdErr.Result}");

            return stdOut.Result.Trim();
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
