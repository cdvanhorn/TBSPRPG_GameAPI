using System;
using System.Threading.Tasks;

using TbspRpgLib.Aggregates;
using TbspRpgLib.Events;

using GameApi.Services;
using GameApi.Entities;

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
            // var dbGame = await _gameService.GetGameById(game.Id);
            // if(dbGame == null) {
            //     throw new Exception("can't process event before game in database");
            // }

            //we're going to send a check event for the game system
            //we're not actually doing anything here, maybe just have the GameSystemService
            //handle it directly and send out a enter_location_check_result event
        }
    }
}