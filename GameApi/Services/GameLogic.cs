using TbspRpgLib.Events;
using GameApi.Adapters;
using GameApi.Entities;

using System;
using System.Threading.Tasks;

namespace GameApi.Services {
    public interface IGameLogic {
        
        Task AddGame(Game game);
        Task<bool> StartGame(string userId, string adventureName);
    }

    public class GameLogic : IGameLogic{
        private IEventAdapter _eventAdapter;
        private IEventService _eventService;
        private IGameService _gameService;
        private IAdventureService _adventureService;

        public GameLogic(
                IEventAdapter eventAdapter,
                IEventService eventService,
                IGameService gameService,
                IAdventureService adventureService)
        {
            _eventAdapter = eventAdapter;
            _eventService = eventService;
            _gameService = gameService;
            _adventureService = adventureService;
        }

        private async Task<bool> AttachAdventureToGame(Game game) {
            if(game.Adventure == null && game.AdventureId != null) {
                Adventure dbadv = await _adventureService.GetAdventureById(game.AdventureId);
                if(dbadv != null) {
                    game.Adventure = dbadv;
                    return true;
                }
            }
            return false;
        }

        public async Task AddGame(Game game) {
            //check if the game already exists
            Game dbGame = await _gameService.GetGameById(game.Id);
            if(dbGame == null && game.AdventureId != null) {
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

        public async Task<bool> StartGame(string userId, string adventureName) {
            Adventure adventure = await _adventureService.GetAdventureByName(adventureName);
            if(adventure == null)
                return false;

            //but bail if there isn't a user id
            Guid uguid;
            if(userId == null || !Guid.TryParse(userId, out uguid))
                return false;

            Game game = await _gameService.GetByUserIdAndAdventureId(uguid, adventure.Id);
            if(game != null)
                return false;
            
            //there isn't an existing game, we'll start a new one
            game = new Game();
            game.Id = Guid.NewGuid();
            game.UserId = uguid;
            game.Adventure = adventure;

            Event newGameEvent = _eventAdapter.NewGameEvent(game);
            _eventService.SendEvent(newGameEvent, true);
            return true;
        }
    }
}