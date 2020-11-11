using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using MongoDB.Bson;

using GameApi.Entities;
using GameApi.Repositories;
using GameApi.Events;

namespace GameApi.Services {
    public interface IGameService {
        Task<List<Game>> GetAll();
        Task<Game> GetByUserIdAndAdventureName(string userid, string name);
        void StartGame(string userId, string adventureName);
    }

    public class GameService : IGameService {
        private IGameRepository _gameRespository;
        private IAdventureService _adventureService;

        public GameService(IGameRepository gameRepository, IAdventureService adventureService) {
            _gameRespository = gameRepository;
            _adventureService = adventureService;
        }

        public Task<List<Game>> GetAll() {
            return _gameRespository.GetAllGames();
        }

        public Task<Game> GetByUserIdAndAdventureName(string userid, string name) {
            return _gameRespository.GetGameByUserIdAndAdventureName(userid, name);
        }

        public async void StartGame(string userId, string adventureName) {
            Task<Adventure> advTask = _adventureService.GetAdventureByName(adventureName);
            Game game = await GetByUserIdAndAdventureName(userId, adventureName);
            if(game != null)
                return;
            
            //there isn't an existing game, we'll start a new one
            game = new Game();
            game.Id = ObjectId.GenerateNewId().ToString();
            game.UserId = userId;
            game.Adventure = await advTask;

            NewGameEvent evnt = new NewGameEvent();
            evnt.InitEvent(game);
            Console.WriteLine(evnt);
        }
    }
}