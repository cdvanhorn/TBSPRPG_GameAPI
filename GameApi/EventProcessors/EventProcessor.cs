using System;
using System.Threading.Tasks;

using GameApi.Adapters;
using GameApi.Entities;
using GameApi.Repositories;
using GameApi.Services;

using TbspRpgLib.EventProcessors;
using TbspRpgLib.Aggregates;
using TbspRpgLib.Settings;
using TbspRpgLib.Events;
using TbspRpgLib.Entities;

using Microsoft.Extensions.DependencyInjection;

namespace GameApi.EventProcessors
{
    public class EventProcessor : MultiEventProcessor
    {
        private IGameAggregateAdapter _gameAdapter;
        private readonly IServiceScopeFactory _scopeFactory;

        public EventProcessor(
            IEventStoreSettings eventStoreSettings,
            IServiceScopeFactory scopeFactory) :
                base(
                    "game",
                    new string[] {
                        Event.NEW_GAME_EVENT_TYPE,
                        Event.ENTER_LOCATION_EVENT_TYPE
                    },
                    eventStoreSettings
                )
        {
            _gameAdapter = new GameAggregateAdapter();
            _scopeFactory = scopeFactory;
            var scope = scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<GameContext>();
            InitializeStartPosition(context);
        }

        protected override async Task HandleEvent(Aggregate aggregate, Event evnt) {
            GameAggregate gameAggregate = (GameAggregate)aggregate;
            Game game = _gameAdapter.ToEntity(gameAggregate);
            EventType eventType = GetEventTypeByName(evnt.Type);
            
            //if the game is missing fields ignore it
            if(game.UserId == null || game.AdventureId == null)
                return;

            using(var scope = _scopeFactory.CreateScope()) {
                var context = scope.ServiceProvider.GetRequiredService<GameContext>();
                var gameLogic = scope.ServiceProvider.GetRequiredService<IGameLogic>();
                var gameService = scope.ServiceProvider.GetRequiredService<IGameService>();

                //check if we've already processed the event
                if(await gameService.HasBeenProcessed(evnt.EventId))
                    return;

                Console.WriteLine($"Writing Game {game.Id} {gameAggregate.GlobalPosition}!!");

                //update the game
                await gameLogic.AddGame(game);
                //update the event type position and this event is processed
                await gameService.UpdatePosition(eventType.Id, gameAggregate.GlobalPosition);
                await gameService.EventProcessed(evnt.EventId);
                //save the changes
                context.SaveChanges();
            }
        }
    }
}
