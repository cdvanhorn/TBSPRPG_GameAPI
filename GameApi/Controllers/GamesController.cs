using Microsoft.AspNetCore.Mvc;

using System.Threading.Tasks;

using GameApi.Services;

namespace GameApi.Controllers {

    [ApiController]
    [Route("/api/[controller]")]
    public class GamesController : ControllerBase {
        //IAdventuresService _adventuresService;
        IGameService _gameService;

        public GamesController(IGameService gameService) {
            _gameService = gameService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var games = await _gameService.GetAll();
            return Ok(games);
        }

        [HttpGet]
        [Authorize]
        public IActionResult GetByAdventure() {
            return Ok();
        }
    }
}