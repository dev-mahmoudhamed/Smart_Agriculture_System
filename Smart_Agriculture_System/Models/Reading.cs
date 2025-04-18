using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Smart_Agriculture_System.Models
{
    public class Reading
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;
        public double Temperature { get; set; }
        public double Humidity { get; set; }
        public string ImageAsBase64 { get; set; } = string.Empty;
        public string Time { get; set; } = string.Empty;
    }

}
