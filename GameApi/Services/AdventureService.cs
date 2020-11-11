using GameApi.Entities;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using GameApi.Repositories;

namespace GameApi.Services {
    public interface IAdventureService {
        Task<List<Adventure>> GetAllAdventures();
        Task<Adventure> GetAdventureByName(string name);
    }

    public class AdventureService : IAdventureService {
        private IAdventureRepository _adventureRespository;

        public AdventureService(IAdventureRepository adventureRepository) {
            _adventureRespository = adventureRepository;
        }

        public Task<List<Adventure>> GetAllAdventures() {
            return _adventureRespository.GetAllAdventures();
        }

        public Task<Adventure> GetAdventureByName(string name) {
            return _adventureRespository.GetAdventureByName(name);
        }
    }
}