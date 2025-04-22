using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Smart_Agriculture_System.Data;
using Smart_Agriculture_System.Models;

namespace Smart_Agriculture_System.Controllers
{
    [ApiController]
    [Route("api/readings")]
    public class ReadingsController : ControllerBase
    {
        private readonly MongoDBContext _context;

        public ReadingsController(MongoDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<SensorReading>> GetAllSensorDataAsync()
        {
            var readings = await _context.SensorReadings
                .Find(_ => true)
                .SortByDescending(r => r.Time)
                .FirstOrDefaultAsync();

            return Ok(readings);
        }

        public async Task<ActionResult<PredictResult>> Predict(PredictInput input)
        {
            throw new NotImplementedException("Prediction logic is not implemented yet.");
        }
    }
}
