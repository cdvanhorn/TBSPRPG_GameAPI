using Microsoft.AspNetCore.Mvc;

using System;
using System.Threading.Tasks;

using GameApi.Services;

namespace GameApi.Controllers {

    [ApiController]
    [Route("/api/[controller]")]
    public class GamesController : ControllerBase {
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

        [HttpGet("{name}")]
        [Authorize]
        public async Task<IActionResult> GetByAdventure(string name) {
            var userId = (string)HttpContext.Items["UserId"];
            var game = await _gameService.GetByUserIdAndAdventureName(userId, name);
            return Ok(game);
        }
    }
}