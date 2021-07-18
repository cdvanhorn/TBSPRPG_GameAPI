using System.Threading.Tasks;
using TbspRpgLib.Aggregates;
using TbspRpgLib.Events;
using GameApi.Adapters;
using GameApi.Services;

namespace GameApi.EventProcessors {
    public interface IEventHandlerServices
    {
        IAdventureService AdventureService { get; }
        IContentService ContentService { get; }
        IGameAggregateAdapter GameAggregateAdapter { get; }
        IGameLogic GameLogic { get; }
        IGameService GameService { get; }
        IEventAdapter EventAdapter { get; }
        IAggregateService AggregateService { get; }
    }

    public class EventHandlerServices : IEventHandlerServices
    {
        public EventHandlerServices(IAdventureService adventureService,
            IContentService contentService,
            IGameLogic gameLogic,
            IGameService gameService,
            IEventAdapter eventAdapter,
            IAggregateService aggregateService)
        {
            GameAggregateAdapter = new GameAggregateAdapter();
            AdventureService = adventureService;
            ContentService = contentService;
            GameLogic = gameLogic;
            GameService = gameService;
            EventAdapter = eventAdapter;
            AggregateService = aggregateService;
        }

        public IAdventureService AdventureService { get; }
        public IContentService ContentService { get; }
        public IGameAggregateAdapter GameAggregateAdapter { get; }
        public IGameLogic GameLogic { get; }
        public IGameService GameService { get; }
        public IEventAdapter EventAdapter { get; }
        public IAggregateService AggregateService { get; }
    }
    
    public interface IEventHandler {
        Task HandleEvent(GameAggregate gameAggregate, Event evnt);
    }

    public class EventHandler
    {
        protected readonly IEventHandlerServices _eventHandlerServices;

        protected EventHandler(IEventHandlerServices eventHandlerServices)
        {
            _eventHandlerServices = eventHandlerServices;
        }
    }
}