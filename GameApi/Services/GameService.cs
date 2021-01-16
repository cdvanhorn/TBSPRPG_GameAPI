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

        public Task<Game> GetByUserIdAndAdventureName(string userid, string name) {
            Guid guid;
            if(!Guid.TryParse(userid, out guid))
                return null;
            return _gameRespository.GetGameByUserIdAndAdventureName(guid, name);
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
