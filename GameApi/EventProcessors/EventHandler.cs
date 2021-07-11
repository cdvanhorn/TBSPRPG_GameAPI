using System.Threading.Tasks;
using TbspRpgLib.Aggregates;
using TbspRpgLib.Events;
using GameApi.Adapters;
using GameApi.Services;

namespace GameApi.EventProcessors {
    public interface IEventHandler {
        Task HandleEvent(GameAggregate gameAggregate, Event evnt);
    }

    public class EventHandler {
        protected readonly IGameAggregateAdapter _gameAdapter;
        protected readonly IContentService _contentService;
        protected readonly IAdventureService _adventureService;

        protected EventHandler(IContentService contentService, IAdventureService adventureService) {
            _gameAdapter = new GameAggregateAdapter();
            _contentService = contentService;
            _adventureService = adventureService;
        }
    }
}