using System;
using System.Linq;
using System.Threading.Tasks;
using GameApi.Entities;
using GameApi.Services;
using TbspRpgLib.Aggregates;
using TbspRpgLib.Events;

namespace GameApi.EventProcessors {
    public interface IGameAddSourceKeyEventHandler : IEventHandler {
        
    }

    public class GameAddSourceKeyEventHandler : EventHandler, IGameAddSourceKeyEventHandler {
        private readonly IGameService _gameService;

        public GameAddSourceKeyEventHandler(IEventHandlerServices eventHandlerServices): base(eventHandlerServices) { }

        public async Task HandleEvent(GameAggregate gameAggregate, Event evnt) {
            var game = _eventHandlerServices.GameAggregateAdapter.ToEntity(gameAggregate);
            //check if the game exists if it doesn't throw an exception
            //we're processing this event before the new_game event
            var dbGame = await _gameService.GetGameById(game.Id);
            if(dbGame == null) {
                throw new Exception("can't process event before game in database");
            }

            //add the given content, we can use the event or the latest source key in the aggregate
            var content = new Content()
            {
                GameId = game.Id,
                Position = evnt.StreamPosition,
                SourceKey = gameAggregate.SourceKeys.Last()
            };
            await _eventHandlerServices.ContentService.AddContent(content);
        }
    }
}