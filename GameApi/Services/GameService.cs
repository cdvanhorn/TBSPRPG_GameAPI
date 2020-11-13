using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using MongoDB.Bson;

using GameApi.Entities;
using GameApi.Repositories;
using GameApi.Events;
using GameApi.Adapters;

namespace GameApi.Services {
    public interface IGameService {
        Task<List<Game>> GetAll();
        Task<Game> GetByUserIdAndAdventureName(string userid, string name);
        void StartGame(string userId, Adventure adventure);
    }

    public class GameService : IGameService {
        private IGameRepository _gameRespository;
        private IAdventureService _adventureService;
        private IEventAdapter _eventAdapter;

        public GameService(IGameRepository gameRepository, IAdventureService adventureService, IEventAdapter eventAdapter) {
            _gameRespository = gameRepository;
            _adventureService = adventureService;
            _eventAdapter = eventAdapter;
        }

        public Task<List<Game>> GetAll() {
            return _gameRespository.GetAllGames();
        }

        public Task<Game> GetByUserIdAndAdventureName(string userid, string name) {
            return _gameRespository.GetGameByUserIdAndAdventureName(userid, name);
        }

        public async void StartGame(string userId, Adventure adventure) {
            Game game = await GetByUserIdAndAdventureName(userId, adventure.Name);
            if(game != null)
                return;
            
            //there isn't an existing game, we'll start a new one
            game = new Game();
            game.Id = ObjectId.GenerateNewId().ToString();
            game.UserId = userId;
            game.Adventure = adventure;

            Event newGameEvent = _eventAdapter.NewGameEvent(game);
        }
    }
}