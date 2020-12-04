using GameApi.Entities;

using MongoDB.Driver;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using TbspRgpLib.Settings;

namespace GameApi.Repositories {
    public interface IGameRepository {
        Task<List<Game>> GetAllGames();
        Task<Game> GetGameByUserIdAndAdventureName(string userid, string name);
        void InsertGameIfDoesntExist(Game game, string eventId);
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

        public Task<Game> GetGameByUserIdAndAdventureName(string userid, string name) {
            return _games.Find(game => 
                userid == game.UserId
                && name.ToLower() == game.Adventure.Name.ToLower()).FirstOrDefaultAsync();
        }

        public void InsertGameIfDoesntExist(Game game, string eventId) {
            //do I care about the mongo write exception, on duplciate
            //events the exception will be thrown.
            var options = new ReplaceOptions { IsUpsert = true };
            var result = _games.ReplaceOneAsync<Game>(
                doc => 
                    doc.UserId == game.UserId 
                    && doc.Adventure.Id == game.Adventure.Id
                    && !doc.Events.Contains(eventId),
                game, options);
            // try {
                
            // } catch (MongoDB.Driver.MongoWriteException) {
            //     //the insert may fail if the game is already there
            //     //this would happen events show up multiple times
            //     Console.WriteLine("Insert Failed  " + game.Id);
            // }

        }
    }
}