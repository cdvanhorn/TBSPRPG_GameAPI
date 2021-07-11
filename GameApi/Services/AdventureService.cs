using GameApi.Entities;

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using GameApi.Entities.AdventureService;
using GameApi.Repositories;
using TbspRpgLib.InterServiceCommunication;
using TbspRpgLib.InterServiceCommunication.RequestModels;

namespace GameApi.Services {
    public interface IAdventureService {
        Task<List<Adventure>> GetAllAdventures();
        Task<Adventure> GetAdventureByName(string name);
        Task<Adventure> GetAdventureById(Guid advId);
        Task<Guid> GetSourceKeyForAdventure(Guid adventureId, Guid userId);
    }

    public class AdventureService : IAdventureService {
        private readonly IAdventureRepository _adventureRepository;
        private readonly IAdventureServiceLink _adventureServiceLink;

        public AdventureService(IAdventureRepository adventureRepository, IAdventureServiceLink adventureServiceLink) {
            _adventureRepository = adventureRepository;
            _adventureServiceLink = adventureServiceLink;
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

        public async Task<Guid> GetSourceKeyForAdventure(Guid adventureId, Guid userId)
        {
            var response = await _adventureServiceLink.GetAdventureById(
                new AdventureRequest()
                {
                    Id = adventureId
                }, new Credentials()
                {
                    UserId = userId.ToString()
                });
            if(!response.IsSuccessful)
                return Guid.Empty;
            
            var adventure = JsonSerializer.Deserialize<AdventureIsc>(
                response.Content,
                new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true
                }
            );

            return adventure.SourceKey;
        }
    }
}