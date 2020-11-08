using GameApi.Entities;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using GameApi.Repositories;

namespace GameApi.Services {
    public interface IGameService {
        Task<List<Game>> GetAll();
        //Task<Adventure> GetByName(string name);
    }

    public class GameService : IGameService {
        private IGameRepository _gameRespository;

        public GameService(IGameRepository gameRepository) {
            _gameRespository = gameRepository;
        }

        public Task<List<Game>> GetAll() {
            return _gameRespository.GetAllGames();
        }

        // public Task<Adventure> GetByName(string name) {
        //     return _adventuresRespository.GetAdventureByName(name);
        // }
    }
}