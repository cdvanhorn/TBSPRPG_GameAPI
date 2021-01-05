using GameApi.Entities;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

using Microsoft.EntityFrameworkCore;

using TbspRpgLib.Settings;
using TbspRpgLib.Repositories;

namespace GameApi.Repositories {
    public interface IAdventureRepository {
        Task<List<Adventure>> GetAllAdventures();
        Task<Adventure> GetAdventureByName(string name);
    }

    public class AdventureRepository : IAdventureRepository {

        public AdventureRepository() {

        }

        public Task<List<Adventure>> GetAllAdventures() {
            //return _adventures.Find(adv => true).ToListAsync();
            return null;
        }

        public Task<Adventure> GetAdventureByName(string name) {
            //return _adventures.Find(adv => adv.Name.ToLower() == name).FirstOrDefaultAsync();
            return null;
        }
    }
}