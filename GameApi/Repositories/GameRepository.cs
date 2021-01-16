using GameApi.Entities;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

using Microsoft.EntityFrameworkCore;

namespace GameApi.Repositories {
    public interface IGameRepository {
        Task<Game> GetGameById(Guid gameId);
        Task<List<Game>> GetAllGames();
        Task<Game> GetGameByUserIdAndAdventureName(Guid userid, string name);
        Task<int> InsertGameIfDoesntExist(Game game);
        void AddGame(Game game);
    }

    public class GameRepository : IGameRepository {
        private GameContext _context;

        public GameRepository(GameContext context) {
            _context = context;
        }

        public Task<List<Game>> GetAllGames() {
            return _context.Games.AsQueryable().ToListAsync();
        }

        public Task<Game> GetGameByUserIdAndAdventureName(Guid userid, string name) {
            return _context.Games.AsQueryable().Include(g => g.Adventure)
                    .Where(g => g.UserId == userid)
                    .Where(g => g.Adventure.Name.ToLower() == name.ToLower())
                    .FirstOrDefaultAsync();
        }

        public Task<Game> GetGameById(Guid gameId) {
            return _context.Games.AsQueryable().Where(g => g.Id == gameId).Include(g => g.Adventure).FirstOrDefaultAsync();
        }

        public async Task<int> InsertGameIfDoesntExist(Game game) {
            var dbGame = await GetGameById(game.Id);
            if(dbGame == null) {
                _context.Games.Add(game);
                var updated = await _context.SaveChangesAsync();
                return updated;
            }
            return 0;
        }

        public void AddGame(Game game) {
            //we assume there is nothing needed to be done to the game
            _context.Games.Add(game);
        }
    }
}