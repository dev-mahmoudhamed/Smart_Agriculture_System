using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Smart_Agriculture_System.Data;
using Smart_Agriculture_System.Models;

namespace Smart_Agriculture_System.Controllers
{
    [ApiController]
    [Route("api/diseases")]
    public class DiseasesController : ControllerBase
    {
        private readonly MongoDBContext _context;

        public DiseasesController(MongoDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Disease>>> GetAllDiseases()
        {
            var diseases = await _context.Diseases.Find(_ => true).ToListAsync();
            return Ok(diseases);
        }

        [HttpPost]
        public async Task<ActionResult<Disease>> CreateDisease(Disease disease)
        {
            await _context.Diseases.InsertOneAsync(disease);
            return CreatedAtAction(nameof(GetAllDiseases), disease);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Disease>> GetDisease(string id)
        {
            var disease = await _context.Diseases.Find(x => x.Id == id).FirstOrDefaultAsync();
            return disease;
        }

    }
}
