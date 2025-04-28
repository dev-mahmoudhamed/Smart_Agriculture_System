﻿using System.Text;
using System.Text.Json.Serialization;

namespace Smart_Agriculture_System.Models
{
    public class GeminiResponce
    {
        [JsonPropertyName("candidates")]
        public List<Candidate> Candidates { get; set; }

        [JsonPropertyName("usageMetadata")]
        public UsageMetadata UsageMetadata { get; set; }

        [JsonPropertyName("modelVersion")]
        public string ModelVersion { get; set; }
    }

    public class Candidate
    {
        [JsonPropertyName("content")]
        public Content Content { get; set; }

        [JsonPropertyName("finishReason")]
        public string FinishReason { get; set; }

        [JsonPropertyName("avgLogprobs")]
        public double AvgLogprobs { get; set; }
    }

    public class Content
    {
        [JsonPropertyName("parts")]
        public List<Part> Parts { get; set; }

        [JsonPropertyName("role")]
        public string Role { get; set; }
    }

    public class Part
    {
        [JsonPropertyName("text")]
        public string Text { get; set; }
    }

    public class UsageMetadata
    {
        [JsonPropertyName("promptTokenCount")]
        public int PromptTokenCount { get; set; }

        [JsonPropertyName("candidatesTokenCount")]
        public int CandidatesTokenCount { get; set; }

        [JsonPropertyName("totalTokenCount")]
        public int TotalTokenCount { get; set; }

        [JsonPropertyName("promptTokensDetails")]
        public List<TokenDetail> PromptTokensDetails { get; set; }

        [JsonPropertyName("candidatesTokensDetails")]
        public List<TokenDetail> CandidatesTokensDetails { get; set; }
    }

    public class TokenDetail
    {
        [JsonPropertyName("modality")]
        public string Modality { get; set; }

        [JsonPropertyName("tokenCount")]
        public int TokenCount { get; set; }
    }

    public class CropPrediction
    {
        [JsonPropertyName("recommended_crop")]
        public string RecommendedCrop { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("confidence")]
        public string Confidence { get; set; }

        [JsonPropertyName("rationale")]
        public string Rationale { get; set; }
        public override string ToString()
        {
            return $"the most suitable plant for cultivation is: {RecommendedCrop ?? "Unknown Crop"}.";
        }
    }
}
