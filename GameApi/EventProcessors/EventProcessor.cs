using System;
using System.Threading.Tasks;
using GameApi.Repositories;
using GameApi.Services;
using Microsoft.Extensions.DependencyInjection;
using TbspRpgLib.Aggregates;
using TbspRpgLib.EventProcessors;
using TbspRpgLib.Events;
using TbspRpgLib.Settings;

namespace GameApi.EventProcessors
{
    public class EventProcessor : MultiEventProcessor
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public EventProcessor(
            IEventStoreSettings eventStoreSettings,
            IServiceScopeFactory scopeFactory) :
                base(
                    "game",
                    new string[] {
                        Event.GAME_NEW_EVENT_TYPE,
                        Event.GAME_ADD_SOURCE_KEY_EVENT_TYPE
                    },
                    eventStoreSettings
                )
        {
            _scopeFactory = scopeFactory;
            var scope = scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<GameContext>();
            InitializeStartPosition(context);
        }

        protected override async Task HandleEvent(Aggregate aggregate, Event evnt) {
            var gameAggregate = (GameAggregate)aggregate;
            var eventType = GetEventTypeByName(evnt.Type);

            using(var scope = _scopeFactory.CreateScope()) {
                var context = scope.ServiceProvider.GetRequiredService<GameContext>();
                var gameService = scope.ServiceProvider.GetRequiredService<IGameService>();
                
                var transaction = context.Database.BeginTransaction();
                try {
                    //check if we've already processed the event
                    if(await gameService.HasBeenProcessed(evnt.EventId))
                        return;

                    //figure out what handler to call based on event type
                    IEventHandler handler = null;
                    if(eventType.TypeName == Event.GAME_NEW_EVENT_TYPE) {
                        handler = scope.ServiceProvider.GetRequiredService<INewGameEventHandler>();
                    } else if(eventType.TypeName == Event.GAME_ADD_SOURCE_KEY_EVENT_TYPE) {
                        handler = scope.ServiceProvider.GetRequiredService<IGameAddSourceKeyEventHandler>();
                    }
                    if(handler != null)
                        await handler.HandleEvent(gameAggregate, evnt);

                    //update the event type position and this event is processed
                    await gameService.UpdatePosition(eventType.Id, gameAggregate.GlobalPosition);
                    await gameService.EventProcessed(evnt.EventId);
                    //save the changes
                    context.SaveChanges();
                    transaction.Commit();
                } catch (Exception) {
                    transaction.Rollback();
                    throw new Exception("event processor error");
                }
            }
        }
    }
}
