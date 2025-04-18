﻿using Microsoft.Extensions.Options;
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

        public IMongoCollection<Soil> Soils => Database.GetCollection<Soil>("soils");
        public IMongoCollection<Plant> Plants => Database.GetCollection<Plant>("plants");
        public IMongoCollection<Disease> Diseases => Database.GetCollection<Disease>("diseases");
        public IMongoCollection<Reading> Readings => Database.GetCollection<Reading>("Readings");
    }

    public class MongoDBSettings
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
        public string CollectionName { get; set; } = string.Empty;

    }
}
