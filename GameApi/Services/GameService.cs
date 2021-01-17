using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using GameApi.Entities;
using GameApi.Repositories;
using GameApi.Adapters;
using GameApi.ViewModels;

using TbspRpgLib.Events;

namespace GameApi.Services {
    public interface IGameService {
        Task<List<Game>> GetAll();
        Task<GameViewModel> GetByUserIdAndAdventureName(string userid, string name);
        Task<Game> GetByUserIdAndAdventureId(Guid userid, Guid adventureId);
        Task<Game> GetGameById(Guid gameId);
        void AddGame(Game game);
    }

    public class GameService : IGameService {
        private IGameRepository _gameRespository;

        public GameService(IGameRepository gameRepository) {
            _gameRespository = gameRepository;
        }

        public Task<List<Game>> GetAll() {
            return _gameRespository.GetAllGames();
        }

        public async Task<GameViewModel> GetByUserIdAndAdventureName(string userid, string name) {
            Guid guid;
            if(!Guid.TryParse(userid, out guid))
                return null;
            var game = await _gameRespository.GetGameByUserIdAndAdventureName(guid, name);
            if(game == null)
                return null;
            return new GameViewModel(game);
        }

        public Task<Game> GetByUserIdAndAdventureId(Guid userid, Guid adventureId) {
            return _gameRespository.GetGameByUserIdAndAdventureId(userid, adventureId);
        }

        public Task<Game> GetGameById(Guid gameId) {
            return _gameRespository.GetGameById(gameId);
        }

        public void AddGame(Game game) {
            _gameRespository.AddGame(game);
        }
    }
}
