using Microsoft.AspNetCore.Mvc;

using System.Threading.Tasks;

namespace GameApi.Controllers {

    [ApiController]
    [Route("/api/[controller]")]
    public class GamesController : ControllerBase {
        //IAdventuresService _adventuresService;

        public GamesController(/*IAdventuresService adventuresService*/) {
            //_adventuresService = adventuresService;
        }

        [HttpGet]
        //public async Task<IActionResult> GetAll()
        public IActionResult GetAll()
        {
            //var adventures = await _adventuresService.GetAll();
            //return Ok(adventures);
            return Ok();
        }
    }
}