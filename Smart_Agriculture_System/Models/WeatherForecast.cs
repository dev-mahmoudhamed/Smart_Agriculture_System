using System.Text.Json.Serialization;

namespace Smart_Agriculture_System.Models
{
    public class WeatherForecastResponse
    {
        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }

        [JsonPropertyName("generationtime_ms")]
        public double GenerationTimeMs { get; set; }

        [JsonPropertyName("utc_offset_seconds")]
        public int UtcOffsetSeconds { get; set; }

        [JsonPropertyName("timezone")]
        public string Timezone { get; set; }

        [JsonPropertyName("timezone_abbreviation")]
        public string TimezoneAbbreviation { get; set; }

        [JsonPropertyName("elevation")]
        public double Elevation { get; set; }

        [JsonPropertyName("hourly_units")]
        public HourlyUnits HourlyUnits { get; set; }

        [JsonPropertyName("hourly")]
        public HourlyData Hourly { get; set; }
    }

    public class HourlyUnits
    {
        [JsonPropertyName("time")]
        public string Time { get; set; }

        [JsonPropertyName("temperature_2m")]
        public string Temperature2M { get; set; }
    }

    public class HourlyData
    {
        [JsonPropertyName("time")]
        public List<string> Time { get; set; } = new List<string>();

        [JsonPropertyName("temperature_2m")]
        public List<double> Temperature2M { get; set; } = new List<double>();
    }
    public class HourlyForecast
    {
        public DateTime Time { get; set; }
        public double Temperature { get; set; }
    }
}
