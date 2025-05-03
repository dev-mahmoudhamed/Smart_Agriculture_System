using Smart_Agriculture_System.Models;
using System.Text;
using System.Text.Json;

namespace Smart_Agriculture_System.Services
{
    public class GeminiServices : IGeminiServices
    {
        private readonly ISensorDataServices _sensorDataServices;
        private readonly HttpClient _httpClient;
        private const string apiKey = "AIzaSyDvPRrxnjAkOrOHz_6mZ4cwC-c28URqekU";
        private const string url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={apiKey}";

        public GeminiServices(HttpClient httpClient, ISensorDataServices sensorDataServices)
        {
            _httpClient = httpClient;
            _sensorDataServices = sensorDataServices;
        }

        public async Task<CropPrediction> Predict(PredictInput input)
        {
            string prompt = @$"You are an expert agronomist AI.
You will receive input detailing environmental parameters:
temperature: {input.Temperature} (in Celsius)
humidity: {input.Humidity} (%)

Analyze these parameters to predict the single best crop to plant under these conditions.

Your response MUST be ONLY a valid JSON object, with no introductory text, explanations, or any text outside the JSON structure.

The JSON object should follow this exact format:
{{
  ""recommended_crop"": ""[Crop Name]"",
  ""description"": ""[Brief description of the crop and its suitability]"",
  ""confidence"": ""[Percentage, e.g., 85%]"".
  ""rationale"": ""[Short explanation linking temperature and humidity to this crop choice]""
}}";

            var response = await GetGeminiTextResponce(prompt);
            string formattedJson = ExtractJson(response);

            var recommendation = JsonSerializer.Deserialize<CropPrediction>(formattedJson, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            return recommendation;
        }

        public async Task<object> AskGemini(ImageRequest request)
        {
            if (request.ImageFile is not null)
            {
                var result = await AnalyzeImage(request);
                return result;
            }
            else
            {
                var result = await GetGeminiTextResponce(request.TextPrompt);
                return result;
            }
        }

        public async Task<string> GetAdvice()
        {
            var sensorData = await _sensorDataServices.GetAllSensorDataAsync();
            var TimeofYear = DateOnly.FromDateTime(DateTime.Now.ToLocalTime()).ToString("MMMM d, yyyy");

            string prompt = $@"Act as an expert agricultural advisor specialized in the Nile Delta region of Egypt.
            I am looking for recommendations on what plants I could successfully start growing now or in the very near future in my location.
                My situation is:
                - Location: Dakahlia Governorate, Egypt.
                - Current Temperature: [{sensorData.Temperature}]°C
                - Current Humidity: [{sensorData.Humidity}]%
                - Time of Year: [{TimeofYear}]
        For each suitable plant, reply **exactly** in this template (one plant per line):
        “{{Plant Name}},  
        {{A very brief note about this plant and its needs to grow well say tips and advices}}”
        Do **not** add any other text.
";

            var response = await GetGeminiTextResponce(prompt);
            var processedResponse = BeautifyAdvice(response);
            return processedResponse;
        }
        private async Task<object> AnalyzeImage(ImageRequest request)
        {
            if (request.ImageFile == null || request.ImageFile.Length == 0)
                return new string("الملف غير موجود أو فارغ!");

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
                throw new Exception($"حدث خطأ أثناء معالجة الصورة: {ex.Message}");
            }

            if (string.IsNullOrEmpty(base64String))
                throw new Exception("Base64 image data is required");

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
                throw new Exception($"Error: {response.StatusCode}, Message: {responseBody}");

            var result = JsonSerializer.Deserialize<object>(responseBody);
            return result;
        }
        private async Task<string> GetGeminiTextResponce(string prompt)
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
                    throw new Exception($"Error: {response.StatusCode}, Message: {responseBody}");

                var result = JsonSerializer.Deserialize<GeminiResponce>(responseBody);
                string resultText = "";
                if (result?.Candidates != null && result.Candidates.Count > 0)
                {
                    var candidate = result.Candidates[0];

                    if (candidate?.Content?.Parts != null && candidate.Content.Parts.Count > 0)
                    {
                        resultText = candidate.Content.Parts[0].Text;
                    }
                }
                return resultText;
            }
            catch (Exception ex)
            {
                throw new Exception($"Exception: {ex.Message}");
            }
        }
        private static string ExtractJson(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            // Find the first '{' and the last '}'
            int startIndex = input.IndexOf('{');
            int endIndex = input.LastIndexOf('}');

            if (startIndex == -1 || endIndex == -1 || endIndex <= startIndex)
                return string.Empty;

            // Extract the JSON substring
            string json = input.Substring(startIndex, endIndex - startIndex + 1);

            return json.Trim();
        }
        private static string BeautifyAdvice(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            var sb = new StringBuilder();

            // Header
            sb.AppendLine("In this time of the year and based on the weather conditions it will be good to plant");
            sb.AppendLine();
            sb.AppendLine();

            var lines = input
                .Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                var parts = line.Split(new[] { ',' }, 2, StringSplitOptions.None);
                if (parts.Length == 2)
                {
                    var name = parts[0].Trim();
                    var desc = parts[1].Trim().TrimEnd('.');

                    // Skip lines with unwanted intro or location
                    if (
                        name.Equals("Ok", StringComparison.OrdinalIgnoreCase) &&
                        desc.StartsWith("based on the information", StringComparison.OrdinalIgnoreCase)
                    )
                    {
                        continue;
                    }

                    // Skip any line that mentions your location (case-insensitive)
                    if (desc.IndexOf("dakahlia", StringComparison.OrdinalIgnoreCase) >= 0 ||
                        desc.IndexOf("egypt", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        continue;
                    }

                    sb.AppendLine($"• {name}: {desc}");
                }
            }

            return sb.ToString();
        }
    }
}
