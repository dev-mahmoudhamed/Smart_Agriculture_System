using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Smart_Agriculture_System.Models;

namespace Smart_Agriculture_System.Data
{
    public class MongoDBContext
    {
        public IMongoDatabase Database { get; }

        public MongoDBContext(IOptions<MongoDBSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            Database = client.GetDatabase(settings.Value.DatabaseName);
        }
        public IMongoCollection<SensorReading> SensorReadings => Database.GetCollection<SensorReading>("sensorReadings");
        public IMongoCollection<ImageReading> ImageReadings => Database.GetCollection<ImageReading>("imageReadings");
    }

    public class MongoDBSettings
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
    }
}
