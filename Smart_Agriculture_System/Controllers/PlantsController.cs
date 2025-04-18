using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Smart_Agriculture_System.Data;
using Smart_Agriculture_System.Models;

namespace Smart_Agriculture_System.Controllers
{
    //[ApiController]
    //[Route("api/plants")]
    public class PlantsController : ControllerBase
    {
        private readonly MongoDBContext _context;

        public PlantsController(MongoDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Plant>>> GetAllPlants()
        {
            var plants = await _context.Plants.Find(_ => true).ToListAsync();
            return Ok(plants);
        }

        [HttpPost]
        public async Task<ActionResult<Plant>> CreatePlant(Plant plant)
        {
            await _context.Plants.InsertOneAsync(plant);
            return CreatedAtAction(nameof(GetAllPlants), plant);
        }
    }
}
