using System.Text.Json.Serialization;

namespace Smart_Agriculture_System.Models
{
    public class CurrentWeather
    {
        [JsonPropertyName("temperature_2m")]
        public double Temperature2M { get; set; }

        [JsonPropertyName("relative_humidity_2m")]
        public double RelativeHumidity2M { get; set; }
    }

    public class WeatherApiResponse
    {
        [JsonPropertyName("current")]
        public CurrentWeather Current { get; set; }
    }


    public class EnvironmentWeather
    {
        [JsonPropertyName("humidity")]
        public string Humidity { get; set; }
        [JsonPropertyName("temperature")]
        public string Temperature { get; set; }

        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }
    }
}
