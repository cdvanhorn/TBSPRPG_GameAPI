using GameApi.Entities;

using MongoDB.Driver;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using TbspRgpLib.Settings;

namespace GameApi.Repositories {
    public interface IGameRepository {
        Task<List<Game>> GetAllGames();
        //Task<Game> GetGameByAdventureName(string name);
    }

    public class GameRepository : IGameRepository {
        private readonly IDatabaseSettings _dbSettings;

        private readonly IMongoCollection<Game> _games;

        public GameRepository(IDatabaseSettings databaseSettings) {
            _dbSettings = databaseSettings;

            var connectionString = $"mongodb+srv://{_dbSettings.Username}:{_dbSettings.Password}@{_dbSettings.Url}/{_dbSettings.Name}?retryWrites=true&w=majority";
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(_dbSettings.Name);

            _games = database.GetCollection<Game>("games");
        }

        public Task<List<Game>> GetAllGames() {
            return _games.Find(game => true).ToListAsync();
        }

        // public Task<Game> GetGameByAdventureName(string name) {
        //     return _adventures.Find(adventure => adventure.Name.ToLower() == name.ToLower()).FirstOrDefaultAsync();
        // }
    }
}