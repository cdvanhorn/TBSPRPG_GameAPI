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

        [Route("start/{adventureId}")]
        [Authorize]
        public async Task<IActionResult> Start(Guid adventureId) {
            Guid userId = Guid.Parse((string)HttpContext.Items[AuthorizeAttribute.USER_ID_CONTEXT_KEY]);
            var success = await _gameLogic.StartGame(userId, adventureId);
            if(!success)
                return BadRequest(new { message = "couldn't start game" });
            return Accepted();
        }

        [HttpGet("{adventureId}")]
        [Authorize]
        public async Task<IActionResult> GetByAdventure(Guid adventureId) {
            Guid userId = Guid.Parse((string)HttpContext.Items[AuthorizeAttribute.USER_ID_CONTEXT_KEY]);
            var game = await _gameService.GetByUserIdAndAdventureIdVm(userId, adventureId);
            return Ok(game);
        }

        [Route("reset")]
        [Authorize]
        public async Task<IActionResult> Reset() {
            await _gameService.ClearData();
            return Accepted();
        }
    }
}
