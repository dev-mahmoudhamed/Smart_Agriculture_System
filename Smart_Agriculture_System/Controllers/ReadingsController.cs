using Microsoft.AspNetCore.Mvc;
using Smart_Agriculture_System.Models;
using Smart_Agriculture_System.Services;
using System.Diagnostics;

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

        // Predict plant that can be planted in this soil at this time of year
        [HttpPost("predict")]
        public async Task<CropPrediction> Predict([FromBody] PredictInput input)
        {
            //try
            //{
            //    excute code of Prediction

            //}
            //catch (Exception ex)
            //{
            var result = await _geminiServices.Predict(input);
            return result;
            //}
        }

        /// <summary>
        ///  Used In adviece page
        /// </summary>
        /// <returns></returns>
        [HttpGet("advices")]
        public async Task<string> GetAdvice()
        {
            var response = await _geminiServices.GetAdvice();
            return response;
        }

        [HttpPost("capture")]
        public async Task<object> CapturePhoneImge(IFormFile img)
        {
            var imgBytes = await ConvertToByteArrayAsync(img);
            ///  do the rest of predict diseases
            /// 
            return null;
        }


        private async Task<byte[]> ConvertToByteArrayAsync(IFormFile formFile)
        {
            var xxx= GetAdvice();
            if (formFile == null || formFile.Length == 0)
                return null;

            using (var memoryStream = new MemoryStream())
            {
                await formFile.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}
