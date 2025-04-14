using Microsoft.AspNetCore.Mvc;
using Smart_Agriculture_System.Models;
using System.Net.Http;
using System.Text;
using System.Text.Json;


namespace Smart_Agriculture_System.Controllers
{

    [ApiController]
    [Route("api/gemini")]
    public class GeminiController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private const string apiKey = "AIzaSyDvPRrxnjAkOrOHz_6mZ4cwC-c28URqekU";
        private const string url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={apiKey}";

        public GeminiController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [HttpPost("askAI")]

        public async Task<IActionResult> AskGemini(ImageRequest request)
        {
            if (request.ImageFile is not null)
            {
                var result = await AnalyzeImage(request);
                return result;
            }
            else
            {
                var result = await AnalyzeText(request.TextPrompt);
                return result;
            }
        }
        
        private async Task<IActionResult> AnalyzeImage(ImageRequest request)
        {
            if (request.ImageFile == null || request.ImageFile.Length == 0)
                return BadRequest("الملف غير موجود أو فارغ!");

            string base64String;
            try
            {
                using var memoryStream = new MemoryStream();
                await request.ImageFile.CopyToAsync(memoryStream);
                byte[] fileBytes = memoryStream.ToArray();

                base64String = Convert.ToBase64String(fileBytes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"حدث خطأ أثناء معالجة الصورة: {ex.Message}");
            }

            if (string.IsNullOrEmpty(base64String))
                return BadRequest("Base64 image data is required.");

            var payload = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new object[]
                        {
                            new { text = request.TextPrompt },
                            new
                            {
                                inline_data = new
                                {
                                    mime_type = "image/jpeg",
                                    data = base64String
                                }
                            }
                        }
                    }
                }
            };

            var jsonPayload = JsonSerializer.Serialize(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");


            var response = await _httpClient.PostAsync(url, content);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, responseBody);

            var result = JsonSerializer.Deserialize<object>(responseBody);
            return Ok(result);
        }
        private async Task<IActionResult> AnalyzeText(string prompt)
        {
            var payload = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                }
            };

            var jsonPayload = JsonSerializer.Serialize(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync(url, content);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    return StatusCode((int)response.StatusCode, responseBody);

                var result = JsonSerializer.Deserialize<object>(responseBody);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Exception: {ex.Message}");
            }
        }
    }
}
