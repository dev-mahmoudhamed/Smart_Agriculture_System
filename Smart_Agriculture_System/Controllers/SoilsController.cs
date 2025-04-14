using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Smart_Agriculture_System.Data;
using Smart_Agriculture_System.Models;

namespace Smart_Agriculture_System.Controllers
{
    [ApiController]
    [Route("api/soils")]
    public class SoilsController : ControllerBase
    {
        private readonly MongoDBContext _context;

        public SoilsController(MongoDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Soil>>> GetAllSoils()
        {
            var soils = await _context.Soils.Find(_ => true).ToListAsync();
            return Ok(soils);
        }

        [HttpPost]
        public async Task<ActionResult<Soil>> CreateSoil(Soil soil)
        {
            await _context.Soils.InsertOneAsync(soil);
            return CreatedAtAction(nameof(GetAllSoils), soil);
        }
    }
}
