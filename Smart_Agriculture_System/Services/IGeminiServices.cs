using Smart_Agriculture_System.Models;

namespace Smart_Agriculture_System.Services
{
    public interface IGeminiServices
    {
        Task<object> AskGemini(ImageRequest request);
        Task<CropPrediction> Predict(PredictInput input);
        Task<string> GetAdvice();
    }
}
