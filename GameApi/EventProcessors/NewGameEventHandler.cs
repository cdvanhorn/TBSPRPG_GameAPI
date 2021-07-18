using System;
using System.Linq;
using System.Threading.Tasks;

using TbspRpgLib.Aggregates;
using TbspRpgLib.Events;

using GameApi.Services;
using GameApi.Entities;

namespace GameApi.EventProcessors {
    public interface INewGameEventHandler : IEventHandler {

    }

    public class NewGameEventHandler : EventHandler, INewGameEventHandler {

        public NewGameEventHandler(IEventHandlerServices eventHandlerServices) : base(eventHandlerServices) { }

        public async Task HandleEvent(GameAggregate gameAggregate, Event evnt)
        {
            var game = _eventHandlerServices.GameAggregateAdapter.ToEntity(gameAggregate);
            Console.WriteLine($"Writing Game {game.Id} {gameAggregate.GlobalPosition}!!");

            //update the game
            await _eventHandlerServices.GameLogic.AddGame(game);

            //create a SourceKey event so content gets added
            var contentEvent = _eventHandlerServices.EventAdapter.GameAddSourceKeyEvent(new Content()
            {
                GameId = game.Id,
                SourceKey = await _eventHandlerServices.AdventureService.GetSourceKeyForAdventure(
                    game.AdventureId, game.UserId)
            });
            await _eventHandlerServices.AggregateService.AppendToAggregate(
                AggregateService.GAME_AGGREGATE_TYPE,
                contentEvent,
                gameAggregate.StreamPosition);
        }
    }
}