using System.Text.Json.Serialization;

namespace Smart_Agriculture_System.Models
{
    public class HealthPredictionResponse
    {
        [JsonPropertyName("class")]
        public string Class { get; set; }

        [JsonPropertyName("confidence")]
        public double Confidence { get; set; }

        [JsonPropertyName("image_url")]
        public string ImageUrl { get; set; }

        [JsonPropertyName("treatment")]
        public string Treatment { get; set; }
    }

}
