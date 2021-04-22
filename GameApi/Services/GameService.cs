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
        private readonly IGameRepository _gameRepository;

        public GameService(IGameRepository gameRepository) : base(gameRepository){
            _gameRepository = gameRepository;
        }

        public Task<List<Game>> GetAll() {
            return _gameRepository.GetAllGames();
        }

        public async Task<GameViewModel> GetByUserIdAndAdventureIdVm(Guid userid, Guid adventureId) {
            var game = await _gameRepository.GetGameByUserIdAndAdventureId(userid, adventureId);
            return game == null ? null : new GameViewModel(game);
        }

        public Task<Game> GetByUserIdAndAdventureId(Guid userid, Guid adventureId) {
            return  _gameRepository.GetGameByUserIdAndAdventureId(userid, adventureId);
        }

        public Task<Game> GetGameById(Guid gameId) {
            return _gameRepository.GetGameById(gameId);
        }

        public void AddGame(Game game) {
            _gameRepository.AddGame(game);
        }

        public async Task ClearData() {
            await _gameRepository.RemoveAllGames();
            await _gameRepository.RemoveAllProcessedEvents();
            await _gameRepository.RemoveAllEventTypePositions();
            _gameRepository.SaveChanges();
        }
    }
}
