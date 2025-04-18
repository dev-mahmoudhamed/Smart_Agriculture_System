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
        public async Task<ActionResult<IEnumerable<Reading>>> GetAllReadings()
        {
            var readings = await _context.Readings.Find(_ => true).ToListAsync();
            return Ok(readings);
        }

        private async Task<ActionResult<Reading>> CreateReading(Reading reading)
        {
            await _context.Readings.InsertOneAsync(reading);
            return CreatedAtAction(nameof(GetAllReadings), reading);
        }
    }
}
