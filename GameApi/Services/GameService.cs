using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using GameApi.Entities;
using GameApi.Repositories;
using GameApi.Adapters;

using TbspRpgLib.Events;

namespace GameApi.Services {
    public interface IGameService {
        Task<List<Game>> GetAll();
        Task<Game> GetByUserIdAndAdventureName(string userid, string name);
        void StartGame(string userId, Adventure adventure);
        Task<Game> GetGameById(Guid gameId);
        void AddGame(Game game);
    }

    public class GameService : IGameService {
        private IGameRepository _gameRespository;
        private IEventAdapter _eventAdapter;
        private IEventService _eventService;

        public GameService(IGameRepository gameRepository, IEventAdapter eventAdapter, IEventService eventService) {
            _gameRespository = gameRepository;
            _eventAdapter = eventAdapter;
            _eventService = eventService;
        }

        public Task<List<Game>> GetAll() {
            return _gameRespository.GetAllGames();
        }

        public Task<Game> GetByUserIdAndAdventureName(string userid, string name) {
            Guid guid;
            if(!Guid.TryParse(userid, out guid))
                return null;
            return _gameRespository.GetGameByUserIdAndAdventureName(guid, name);
        }

        public Task<Game> GetGameById(Guid gameId) {
            return _gameRespository.GetGameById(gameId);
        }

        public void AddGame(Game game) {
            _gameRespository.AddGame(game);
        }

        public async void StartGame(string userId, Adventure adventure) {
            //we assume the adventure is valid
            //but bail if there isn't a user id
            Guid uguid;
            if(userId == null || !Guid.TryParse(userId, out uguid))
                return;

            Game game = await GetByUserIdAndAdventureName(userId, adventure.Name);
            if(game != null)
                return;
            
            //there isn't an existing game, we'll start a new one
            game = new Game();
            game.Id = Guid.NewGuid();
            game.UserId = uguid;
            game.Adventure = adventure;

            Event newGameEvent = _eventAdapter.NewGameEvent(game);
            _eventService.SendEvent(newGameEvent, true);
        }
    }
}
