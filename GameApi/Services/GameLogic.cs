using TbspRpgLib.Events;
using GameApi.Adapters;
using GameApi.Entities;

using System.Threading.Tasks;

namespace GameApi.Services {
    public interface IGameLogic {
        
        Task AddGame(Game game);
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
    }
}