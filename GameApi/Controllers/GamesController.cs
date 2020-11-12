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
        //[Authorize]
        public async Task<IActionResult> GetAll()
        {
            var games = await _gameService.GetAll();
            return Ok(games);
        }

        [Route("start/{name}")]
        //[Authorize]
        public async Task<IActionResult> Start(string name) {
            //make sure the name is valid before we kick things off and leave
            var adventure = await _adventureService.GetAdventureByName(name);
            if(adventure == null)
                return BadRequest(new { message = "invalid adventure name" });

            var userId = (string)HttpContext.Items["UserId"];
            //var userId = "5fa8585243485e01dc847b8c";
            //var userId = "5fa8585243485e01dc847b8b";
            _gameService.StartGame(userId, adventure);
            return Accepted();
        }

        [HttpGet("{name}")]
        //[Authorize]
        public async Task<IActionResult> GetByAdventure(string name) {
            // var settings = new EventStoreClientSettings {
            //     ConnectivitySettings = {
            //         Address = new Uri("http://eventstore:2113")
            //     }
            // };
            // var client = new EventStoreClient(settings);
            // var eventData = new EventData(
            //     Uuid.NewUuid(),
            //     "some-event",
            //     Encoding.UTF8.GetBytes("{\"id\": \"1\" \"value\": \"some value\"}")
            // );

            // await client.AppendToStreamAsync(
            //     "some-stream",
            //     StreamState.Any,
            //     new List<EventData> {
            //         eventData
            //     });

            // var events = client.ReadStreamAsync(
            //     Direction.Forwards,
            //     "some-stream",
            //     StreamPosition.Start,
            //     1);

            // await foreach (var @event in events) {
            //     Console.WriteLine(Encoding.UTF8.GetString(@event.Event.Data.Span));
            // }

            var userId = (string)HttpContext.Items["UserId"];
            var game = await _gameService.GetByUserIdAndAdventureName(userId, name);
            return Ok(game);
        }
    }
}