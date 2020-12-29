using GameApi.Entities;

using MongoDB.Driver;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using TbspRpgLib.Settings;
using TbspRpgLib.Repositories;

namespace GameApi.Repositories {
    public interface IAdventureRepository {
        Task<List<Adventure>> GetAllAdventures();
        Task<Adventure> GetAdventureByName(string name);
    }

    public class AdventureRepository : MongoRepository, IAdventureRepository {
        private readonly IMongoCollection<Adventure> _adventures;

        public AdventureRepository(IDatabaseSettings databaseSettings) : base(databaseSettings){
            _adventures = _mongoDatabase.GetCollection<Adventure>("adventures");
        }

        public Task<List<Adventure>> GetAllAdventures() {
            return _adventures.Find(adv => true).ToListAsync();
        }

        public Task<Adventure> GetAdventureByName(string name) {
            return _adventures.Find(adv => adv.Name.ToLower() == name).FirstOrDefaultAsync();
        }
    }
}