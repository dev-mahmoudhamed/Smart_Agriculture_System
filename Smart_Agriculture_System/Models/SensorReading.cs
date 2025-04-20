using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Smart_Agriculture_System.Models
{
    public class SensorReading
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;
        public double Temperature { get; set; }
        public double Humidity { get; set; }
        public DateTime Time { get; set; }
    }


    public class ImageReading
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;
        public string ImageAsBase64 { get; set; } = string.Empty;
        public DateTime Time { get; set; }
    }
}