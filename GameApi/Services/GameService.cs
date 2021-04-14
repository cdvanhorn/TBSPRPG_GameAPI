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
        Task<GameViewModel> GetByUserIdAndAdventureIdVm(Guid userid, Guid adventureId);
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

        public async Task<GameViewModel> GetByUserIdAndAdventureIdVm(Guid userid, Guid adventureId) {
            var game = await _gameRespository.GetGameByUserIdAndAdventureId(userid, adventureId);
            return game == null ? null : new GameViewModel(game);
        }

        public Task<Game> GetByUserIdAndAdventureId(Guid userid, Guid adventureId) {
            return  _gameRespository.GetGameByUserIdAndAdventureId(userid, adventureId);
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
