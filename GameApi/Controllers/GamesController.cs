using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using EventStore.Client;

using GameApi.Services;

namespace GameApi.Controllers {

    [ApiController]
    [Route("/api/[controller]")]
    public class GamesController : ControllerBase {
        IGameService _gameService;
        IGameLogic _gameLogic;

        public GamesController(IGameService gameService, IGameLogic gameLogic) {
            _gameService = gameService;
            _gameLogic = gameLogic;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var games = await _gameService.GetAll();
            return Ok(games);
        }

        [Route("start/{name}")]
        [Authorize]
        public async Task<IActionResult> Start(string name) {
            var userId = (string)HttpContext.Items[AuthorizeAttribute.USER_ID_CONTEXT_KEY];
            var success = await _gameLogic.StartGame(userId, name);
            if(!success)
                return BadRequest(new { message = "couldn't start game" });
            return Accepted();
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