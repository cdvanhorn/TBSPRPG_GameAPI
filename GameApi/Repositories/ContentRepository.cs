using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameApi.Entities;
using Microsoft.EntityFrameworkCore;
using TbspRpgLib.Repositories;

namespace GameApi.Repositories {
    public interface IContentRepository : IServiceTrackingRepository
    {
        Task<List<Content>> GetContentForGame(Guid gameId, int? offset = null, int? count = null);
        Task<List<Content>> GetContentForGameReverse(Guid gameId, int? offset = null, int? count = null);
        void AddContent(Content content);
        void SaveChanges();
        Task<Content> GetContentForGameWithPosition(Guid gameId, ulong position);
        Task<List<Content>> GetContentForGameAfterPosition(Guid gameId, ulong position);
    }

    public class ContentRepository : ServiceTrackingRepository, IContentRepository {
        private readonly GameContext _context;

        public ContentRepository(GameContext context) : base(context) {
            _context = context;
        }

        public Task<List<Content>> GetContentForGame(Guid gameId, int? offset = null, int? count = null)
        {
            var query = _context.Contents.AsQueryable()
                .Where(c => c.GameId == gameId)
                .OrderBy(c => c.Position);
            if (offset != null)
                query = (IOrderedQueryable<Content>) query.Skip(offset.Value);
            if (count != null)
                query = (IOrderedQueryable<Content>) query.Take(count.Value);
            return query.ToListAsync();
        }

        public Task<List<Content>> GetContentForGameReverse(Guid gameId, int? offset = null, int? count = null)
        {
            var query = _context.Contents.AsQueryable()
                .Where(c => c.GameId == gameId)
                .OrderByDescending(c => c.Position);
            if (offset != null)
                query = (IOrderedQueryable<Content>) query.Skip(offset.Value);
            if (count != null)
                query = (IOrderedQueryable<Content>) query.Take(count.Value);
            return query.ToListAsync();
        }

        public void AddContent(Content content)
        {
            _context.Contents.Add(content);
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public Task<Content> GetContentForGameWithPosition(Guid gameId, ulong position)
        {
            return _context.Contents.AsQueryable()
                .FirstOrDefaultAsync(c => c.GameId == gameId && c.Position == position);
        }

        public Task<List<Content>> GetContentForGameAfterPosition(Guid gameId, ulong position)
        {
            return _context.Contents.AsQueryable()
                .Where(c => c.GameId == gameId && c.Position > position)
                .OrderBy(c => c.Position).ToListAsync();
        }
    }
}
