using MongoDB.Driver;
using Smart_Agriculture_System.Data;
using Smart_Agriculture_System.Models;

namespace Smart_Agriculture_System.Services
{
    public class SensorDataServices : ISensorDataServices
    {
        private readonly MongoDBContext _context;

        public SensorDataServices(MongoDBContext context)
        {
            _context = context;
        }

        public async Task<SensorReading> GetAllSensorDataAsync()
        {
            var readings = await _context.SensorReadings
                .Find(_ => true)
                .SortByDescending(r => r.Time)
                .FirstOrDefaultAsync();

            return readings;
        }
    }
}
