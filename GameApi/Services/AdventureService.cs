using GameApi.Entities;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using GameApi.Repositories;

namespace GameApi.Services {
    public interface IAdventureService {
        Task<List<Adventure>> GetAllAdventures();
        Task<Adventure> GetAdventureByName(string name);
        Task<Adventure> GetAdventureById(Guid advId);
    }

    public class AdventureService : IAdventureService {
        private readonly IAdventureRepository _adventureRepository;

        public AdventureService(IAdventureRepository adventureRepository) {
            _adventureRepository = adventureRepository;
        }

        public Task<List<Adventure>> GetAllAdventures() {
            return _adventureRepository.GetAllAdventures();
        }

        public Task<Adventure> GetAdventureByName(string name) {
            return _adventureRepository.GetAdventureByName(name);
        }

        public Task<Adventure> GetAdventureById(Guid advId) {
            return _adventureRepository.GetAdventureById(advId);
        }
    }
}