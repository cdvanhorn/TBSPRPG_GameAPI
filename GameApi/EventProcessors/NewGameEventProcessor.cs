using System;

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
        private IGameAggregateAdapter _gameAdapter;
        private readonly IServiceScopeFactory _scopeFactory;

        public MyNewGameEventProcessor(IEventStoreSettings eventStoreSettings, IServiceScopeFactory scopeFactory) :
            base("game", eventStoreSettings){
            _gameAdapter = new GameAggregateAdapter();
            _scopeFactory = scopeFactory;
            var scope = scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<GameContext>();
            InitializeStartPosition(context);
        }

        protected override async void HandleEvent(Aggregate aggregate, string eventId, ulong position) {
            GameAggregate gameAggregate = (GameAggregate)aggregate;
            Game game = _gameAdapter.ToEntity(gameAggregate);
            
            //if the game is missing fields ignore it
            if(game.UserId == null || game.AdventureId == null)
                return;

            //make sure the event id is a valid guid
            Guid eventguid;
            if(!Guid.TryParse(eventId, out eventguid))
                return;

            using(var scope = _scopeFactory.CreateScope()) {
                var context = scope.ServiceProvider.GetRequiredService<GameContext>();
                var gameLogic = scope.ServiceProvider.GetRequiredService<IGameLogic>();
                var gameService = scope.ServiceProvider.GetRequiredService<IGameService>();

                //check if we've already processed the events
                if(await gameService.HasBeenProcessed(_service.Id, eventguid))
                    return;

                Console.WriteLine($"Writing Game {game.Id} {position}!!");

                //update the game
                await gameLogic.AddGame(game);
                //update the event type position and this event is processed
                await gameService.UpdatePosition(_service.Id, _eventType.Id, position);
                await gameService.EventProcessed(_service.Id, eventguid);
                //save the changes
                Console.WriteLine(context.SaveChanges());
            }
        }
    }
}
