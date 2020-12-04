using GameApi.Entities;

using MongoDB.Driver;

using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using TbspRgpLib.Settings;

namespace GameApi.Repositories {
    public interface IServiceRespository {
        Task<List<Service>> GetAllServices();

        Task<Service> GetServiceByName(string name);

        void UpdateService(Service service, string eventName);
    }

    public class ServiceRepository : IServiceRespository {
        private readonly IDatabaseSettings _dbSettings;

        private readonly IMongoCollection<Service> _services;

        public ServiceRepository(IDatabaseSettings databaseSettings) {
            _dbSettings = databaseSettings;

            var connectionString = $"mongodb+srv://{_dbSettings.Username}:{_dbSettings.Password}@{_dbSettings.SystemDatabaseUrl}/{_dbSettings.SystemDatabaseName}?retryWrites=true&w=majority";
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(_dbSettings.SystemDatabaseName);

            _services = database.GetCollection<Service>("services");
        }

        public Task<List<Service>> GetAllServices() {
            return _services.Find(service => true).ToListAsync();
        }

        public Task<Service> GetServiceByName(string name) {
            return _services.Find(service => 
                name.ToLower() == service.Name.ToLower()).FirstOrDefaultAsync();
        }

        public void UpdateService(Service service, string eventName) {
            var filter = Builders<Service>.Filter.Eq(doc => doc.Id, service.Id)
                & Builders<Service>.Filter.ElemMatch(doc => doc.EventIndexes, 
                    Builders<EventIndex>.Filter.Eq(ei => ei.EventName, eventName));
            var update = Builders<Service>.Update.Set(doc => doc.EventIndexes[-1].Index,
                service.EventIndexes.Where(ei => ei.EventName == eventName).First().Index
            );
            var result = _services.UpdateOneAsync(filter, update);
        }
    }
}