using System.Threading.Tasks;
using TbspRpgLib.Aggregates;
using TbspRpgLib.Events;
using GameApi.Adapters;

namespace GameApi.EventProcessors {
    public interface IEventHandler {
        Task HandleEvent(GameAggregate gameAggregate, Event evnt);
    }

    public class EventHandler {
        protected readonly IGameAggregateAdapter _gameAdapter;

        protected EventHandler() {
            _gameAdapter = new GameAggregateAdapter();
        }
    }
}