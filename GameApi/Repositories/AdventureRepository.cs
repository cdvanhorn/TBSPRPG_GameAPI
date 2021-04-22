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
        Task<Adventure> GetAdventureById(Guid advId);
    }

    public class AdventureRepository : IAdventureRepository {
        private readonly GameContext _context;

        public AdventureRepository(GameContext context) {
            _context = context;
        }

        public Task<List<Adventure>> GetAllAdventures() {
            return _context.Adventures.AsQueryable().ToListAsync();
        }

        public Task<Adventure> GetAdventureByName(string name)
        {
            if (name == null)
                return Task.FromResult<Adventure>(null);
            return _context.Adventures.AsQueryable()
                    .Where(adv => adv.Name.ToLower() == name.ToLower())
                    .FirstOrDefaultAsync();
        }

        public Task<Adventure> GetAdventureById(Guid advId) {
            return _context.Adventures.AsQueryable().Where(adv => adv.Id == advId).FirstOrDefaultAsync();
        }
    }
}