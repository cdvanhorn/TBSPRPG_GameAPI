using GameApi.Entities;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

using Microsoft.EntityFrameworkCore;

namespace GameApi.Repositories {
    public interface IAdventureRepository {
        Task<List<Adventure>> GetAllAdventures();
        Task<Adventure> GetAdventureByName(string name);
    }

    public class AdventureRepository : IAdventureRepository {
        private GameContext _context;

        public AdventureRepository(GameContext context) {
            _context = context;
        }

        public Task<List<Adventure>> GetAllAdventures() {
            return _context.Adventures.AsQueryable().ToListAsync();
        }

        public Task<Adventure> GetAdventureByName(string name) {
            return _context.Adventures.AsQueryable()
                    .Where(adv => adv.Name.ToLower() == name.ToLower())
                    .FirstOrDefaultAsync();
        }
    }
}