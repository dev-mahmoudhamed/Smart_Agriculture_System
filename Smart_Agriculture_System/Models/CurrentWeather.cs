using System.Text.Json.Serialization;

namespace Smart_Agriculture_System.Models
{
    public class CurrentWeather
    {
        [JsonPropertyName("temperature_2m")]
        public double Temperature2M { get; set; }

        [JsonPropertyName("relative_humidity_2m")]
        public int RelativeHumidity2M { get; set; }
    }

    public class WeatherApiResponse
    {
        [JsonPropertyName("current")]
        public CurrentWeather Current { get; set; }
    }

}
