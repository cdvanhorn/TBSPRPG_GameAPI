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
        IAdventureService _adventureService;

        public GamesController(IGameService gameService, IAdventureService adventureService) {
            _gameService = gameService;
            _adventureService = adventureService;
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
            //make sure the name is valid before we kick things off and leave
            var adventure = await _adventureService.GetAdventureByName(name);
            if(adventure == null)
                return BadRequest(new { message = "invalid adventure name" });

            var userId = (string)HttpContext.Items[AuthorizeAttribute.USER_ID_CONTEXT_KEY];
            _gameService.StartGame(userId, adventure);
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