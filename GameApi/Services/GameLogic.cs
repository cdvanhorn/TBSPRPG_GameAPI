using TbspRpgLib.Events;
using TbspRpgLib.Aggregates;
using GameApi.Adapters;
using GameApi.Entities;

using System;
using System.Threading.Tasks;

namespace GameApi.Services {
    public interface IGameLogic {
        
        Task AddGame(Game game);
        Task<bool> StartGame(Guid userId, Guid adventureId);
    }

    public class GameLogic : IGameLogic{
        private readonly IEventAdapter _eventAdapter;
        private readonly IAggregateService _aggregateService;
        private readonly IGameService _gameService;
        private readonly IAdventureService _adventureService;

        public GameLogic(
                IEventAdapter eventAdapter,
                IAggregateService aggService,
                IGameService gameService,
                IAdventureService adventureService)
        {
            _eventAdapter = eventAdapter;
            _aggregateService = aggService;
            _gameService = gameService;
            _adventureService = adventureService;
        }

        private async Task<bool> AttachAdventureToGame(Game game) {
            if (game.Adventure != null || game.AdventureId == null) return false;
            var dbadv = await _adventureService.GetAdventureById(game.AdventureId);
            if (dbadv == null) return false;
            game.Adventure = dbadv;
            return true;
        }

        public async Task AddGame(Game game) {
            //check if the game already exists
            var dbGame = await _gameService.GetGameById(game.Id);
            if(dbGame == null) {
                //the game doesn't exist add it
                //attach the adventure object
                var attached = await AttachAdventureToGame(game);
                if(game.Adventure == null) {
                    //we couldn't find or where unable to attach the adventure
                    return;
                }
                _gameService.AddGame(game);
            }
        }

        public async Task<bool> StartGame(Guid userId, Guid adventureId) {
            var adventure = await _adventureService.GetAdventureById(adventureId);
            if(adventure == null)
                return false;

            var game = await _gameService.GetByUserIdAndAdventureId(userId, adventure.Id);
            if(game != null)
                return true;  //do nothing if the game already exists
            
            //there isn't an existing game, we'll start a new one
            game = new Game
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Adventure = adventure
            };

            var newGameEvent = _eventAdapter.NewGameEvent(game);
            await _aggregateService.AppendToAggregate(
                AggregateService.GAME_AGGREGATE_TYPE,
                newGameEvent,
                true)
            ;
            return true;
        }
    }
}