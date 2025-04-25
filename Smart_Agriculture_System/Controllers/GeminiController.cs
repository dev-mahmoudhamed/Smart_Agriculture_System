using Microsoft.AspNetCore.Mvc;
using Smart_Agriculture_System.Models;
using Smart_Agriculture_System.Services;


namespace Smart_Agriculture_System.Controllers
{

    [ApiController]
    [Route("api/gemini")]
    public class GeminiController : ControllerBase
    {
        private readonly IGeminiServices _geminiServices;

        public GeminiController(IGeminiServices geminiServices)
        {
            _geminiServices = geminiServices;
        }

        [HttpPost("askAI")]

        public async Task<IActionResult> AskGemini(ImageRequest request)
        {
            var response = await _geminiServices.AskGemini(request);
            return Ok(response);
        }

    }
}
