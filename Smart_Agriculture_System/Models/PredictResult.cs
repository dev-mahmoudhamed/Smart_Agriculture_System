namespace Smart_Agriculture_System.Models
{
    public class PredictResult
    {
        public string Base64Image { get; set; } = string.Empty;
        public string TextResult { get; set; } = string.Empty;
    }


    public class PredictInput
    {
        public double Temperature { get; set; }
        public double Humidity { get; set; }
    }
}
