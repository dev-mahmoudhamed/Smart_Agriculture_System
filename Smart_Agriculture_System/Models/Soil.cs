using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Smart_Agriculture_System.Models
{
    public class Soil
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public double Temperature { get; set; }
        public double Humidity { get; set; }
        public double PH { get; set; }
    }

    public class Plant
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public List<string> DiseaseIds { get; set; } = new List<string>();
    }

    public class Disease
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Symptoms { get; set; }
        public string Treatment { get; set; }
    }
}
