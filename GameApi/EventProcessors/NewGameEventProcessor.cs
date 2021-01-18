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

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GameApi.EventProcessors
{
    public class MyNewGameEventProcessor : NewGameEventProcessor
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public MyNewGameEventProcessor(IEventStoreSettings eventStoreSettings, IServiceScopeFactory scopeFactory) :
            base("game", eventStoreSettings){
            _scopeFactory = scopeFactory;
            var scope = _scopeFactory.CreateScope();
            this.InitializeServices(
                scope.ServiceProvider.GetRequiredService<GameContext>()
            );
        }

        protected override async void HandleEvent(Aggregate aggregate, string eventId, ulong position) {
            GameAggregate gameAggregate = (GameAggregate)aggregate;
            GameAggregateAdapter _gameAdapter = new GameAggregateAdapter();
            Game game = _gameAdapter.ToEntity(gameAggregate);

            using(var scope = _scopeFactory.CreateScope()) {
                var context = scope.ServiceProvider.GetRequiredService<GameContext>();
                var gameLogic = scope.ServiceProvider.GetRequiredService<IGameLogic>();
                InitializeServices(context);

                //if the game is missing fields ignore it
                if(game.UserId == null || game.AdventureId == null)
                    return;
                Console.WriteLine($"Writing Game {game.Id} {position} {eventId}!!");

                //make sure the event id is a valid guid
                Guid eventguid;
                if(!Guid.TryParse(eventId, out eventguid))
                    return;

                //update the game
                await gameLogic.AddGame(game);
                //update the event type position
                await UpdatePosition(position);
                //update the processed events
                await AddEventTracked(eventId);
                //save the changes
                context.SaveChanges();
            }
        }
    }
}
