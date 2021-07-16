using System;
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
            
            //add content
            var content = new Content()
            {
                GameId = game.Id,
                Position = evnt.StreamPosition,
                SourceKey = await _eventHandlerServices.AdventureService.GetSourceKeyForAdventure(game.AdventureId, game.UserId)
            };
            await _eventHandlerServices.ContentService.AddContent(content);
        }
    }
}