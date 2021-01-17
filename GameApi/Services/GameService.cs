using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using GameApi.Entities;
using GameApi.Repositories;
using GameApi.ViewModels;

using TbspRpgLib.Services;

namespace GameApi.Services {
    public interface IGameService : IServiceTrackingService{
        Task<List<Game>> GetAll();
        Task<GameViewModel> GetByUserIdAndAdventureName(string userid, string name);
        Task<Game> GetByUserIdAndAdventureId(Guid userid, Guid adventureId);
        Task<Game> GetGameById(Guid gameId);
        void AddGame(Game game);
        Task ClearData();
    }

    public class GameService : ServiceTrackingService, IGameService {
        private IGameRepository _gameRespository;

        public GameService(IGameRepository gameRepository) : base(gameRepository){
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

        public async Task ClearData() {
            await _gameRespository.RemoveAllGames();
            await _gameRespository.RemoveAllProcessedEvents();
            await _gameRespository.RemoveAllEventTypePositions();
            _gameRespository.SaveChanges();
        }
    }
}
