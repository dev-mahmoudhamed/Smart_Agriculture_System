using Microsoft.AspNetCore.Mvc;
using Smart_Agriculture_System.Models;
using System.Text.Json;

namespace Smart_Agriculture_System.Controllers
{

    //[ApiController]
    //[Route("api/[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public WeatherForecastController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> GetHourlyTemperature([FromQuery] double latitude = 31.0539, [FromQuery] double longitude = 31.3779)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                var apiUrl = $"https://api.open-meteo.com/v1/forecast?" +
                              $"latitude={latitude}&" +
                              $"longitude={longitude}&" +
                              "hourly=temperature_2m&" +
                              "forecast_days=1&" +
                              "timezone=auto";

                var response = await httpClient.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode();

                var jsonString = await response.Content.ReadAsStringAsync();
                var weatherData = JsonSerializer.Deserialize<WeatherForecastResponse>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                var hourlyData = weatherData.Hourly.Time
                    .Zip(weatherData.Hourly.Temperature2M, (time, temp) => new HourlyForecast
                    {
                        Time = DateTime.Parse(time),
                        Temperature = temp
                    }).ToList();


                return Ok(hourlyData);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, $"Error fetching weather data: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
