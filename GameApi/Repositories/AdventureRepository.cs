using GameApi.Entities;

using MongoDB.Driver;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using TbspRpgLib.Settings;

namespace GameApi.Repositories {
    public interface IAdventureRepository {
        Task<List<Adventure>> GetAllAdventures();
        Task<Adventure> GetAdventureByName(string name);
    }

    public class AdventureRepository : IAdventureRepository {
        private readonly IDatabaseSettings _dbSettings;

        private readonly IMongoCollection<Adventure> _adventures;

        public AdventureRepository(IDatabaseSettings databaseSettings) {
            _dbSettings = databaseSettings;

            var connectionString = $"mongodb+srv://{_dbSettings.Username}:{_dbSettings.Password}@{_dbSettings.Url}/{_dbSettings.Name}?retryWrites=true&w=majority";
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(_dbSettings.Name);

            _adventures = database.GetCollection<Adventure>("adventures");
        }

        public Task<List<Adventure>> GetAllAdventures() {
            return _adventures.Find(adv => true).ToListAsync();
        }

        public Task<Adventure> GetAdventureByName(string name) {
            return _adventures.Find(adv => adv.Name.ToLower() == name).FirstOrDefaultAsync();
        }
    }
}