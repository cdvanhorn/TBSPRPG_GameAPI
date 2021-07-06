using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameApi.Entities;
using GameApi.Repositories;
using TbspRpgLib.Services;

namespace GameApi.Services {
    public interface IContentService : IServiceTrackingService {
        Task<List<Content>> GetAllContentForGame(Guid gameId);
        Task<Content> GetLatestForGame(Guid gameId);
        Task<List<Content>> GetContentForGameAfterPosition(Guid gameId, ulong position);
        Task<List<Content>> GetPartialContentForGame(Guid gameId, string direction, int? offset, int? count);
        Task AddContent(Content content);
    }

    public class ContentService : ServiceTrackingService, IContentService {
        private readonly IContentRepository _repository;

        public ContentService(IContentRepository repository) : base(repository) {
            _repository = repository;
        }

        public async Task<List<Content>> GetAllContentForGame(Guid gameId)
        {
            var contents = await _repository.GetContentForGame(gameId);
            return contents;
        }

        public async Task<Content> GetLatestForGame(Guid gameId)
        {
            var contents = await _repository.GetContentForGameReverse(gameId, null, 1);
            return contents.FirstOrDefault();
        }
        
        public async Task<List<Content>> GetContentForGameAfterPosition(Guid gameId, ulong position)
        {
            return await _repository.GetContentForGameAfterPosition(gameId, position);
        }

        public async Task<List<Content>> GetPartialContentForGame(Guid gameId, string direction, int? offset, int? count)
        {
            List<Content> contents;
            if (string.IsNullOrEmpty(direction) || direction.ToLower()[0] == 'f')
            {
                contents = await _repository.GetContentForGame(
                    gameId, offset, count);
            } 
            else if (direction.ToLower()[0] == 'b')
            {
                contents = await _repository.GetContentForGameReverse(
                    gameId, offset, count);
            }
            else
            {
                //we can't parse the direction
                throw new ArgumentException($"invalid direction argument {direction}");
            }

            return contents;
        }

        public async Task AddContent(Content content)
        {
            //check if we already have this content determined by game id and position
            var dbContent = await _repository.GetContentForGameWithPosition(content.GameId, content.Position);
            if (dbContent == null)
            {
                _repository.AddContent(content);
            }
        }
    }
}
