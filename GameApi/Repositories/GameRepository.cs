using GameApi.Entities;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

using Microsoft.EntityFrameworkCore;

using TbspRpgLib.Repositories;

namespace GameApi.Repositories {
    public interface IGameRepository : IServiceTrackingRepository {
        Task<Game> GetGameById(Guid gameId);
        Task<List<Game>> GetAllGames();
        Task<Game> GetGameByUserIdAndAdventureId(Guid userid, Guid adventureId);
        void AddGame(Game game);
        Task RemoveAllGames();
        void SaveChanges();
    }

    public class GameRepository : ServiceTrackingRepository, IGameRepository {
        private readonly GameContext _context;

        public GameRepository(GameContext context) : base(context){
            _context = context;
        }

        public Task<List<Game>> GetAllGames() {
            return _context.Games.AsQueryable().ToListAsync();
        }
        
        public Task<Game> GetGameByUserIdAndAdventureId(Guid userid, Guid adventureId) {
            return _context.Games.AsQueryable().Include(g => g.Adventure)
                    .Where(g => g.UserId == userid)
                    .Where(g => g.AdventureId == adventureId)
                    .FirstOrDefaultAsync();
        }

        public Task<Game> GetGameById(Guid gameId) {
            return _context.Games.AsQueryable().Where(g => g.Id == gameId).Include(g => g.Adventure).FirstOrDefaultAsync();
        }

        public void AddGame(Game game) {
            //we assume there is nothing needed to be done to the game
            _context.Games.Add(game);
        }

        public async Task RemoveAllGames() {
            //clear out the games
            var games = await GetAllGames();
            _context.Games.RemoveRange(games);
        }

        public void SaveChanges() {
            _context.SaveChanges();
        }
    }
}