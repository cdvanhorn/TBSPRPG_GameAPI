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
        private IGameLogic _gameLogic;
        private GameContext _context;
        private readonly IServiceScope _scope;

        public MyNewGameEventProcessor(IEventStoreSettings eventStoreSettings, IServiceScopeFactory scopeFactory) :
            base("game", eventStoreSettings){
            _gameAdapter = new GameAggregateAdapter();
            _scope = scopeFactory.CreateScope();
            _context = _scope.ServiceProvider.GetRequiredService<GameContext>();
            _gameLogic = _scope.ServiceProvider.GetRequiredService<IGameLogic>();
            this.InitializeServices(_context);
        }

        protected override async void HandleEvent(Aggregate aggregate, string eventId, ulong position) {
            GameAggregate gameAggregate = (GameAggregate)aggregate;
            Game game = _gameAdapter.ToEntity(gameAggregate);
            
            //if the game is missing fields ignore it
            if(game.UserId == null || game.AdventureId == null)
                return;
            Console.WriteLine($"Writing Game {game.Id} {position}!!");

            //make sure the event id is a valid guid
            Guid eventguid;
            if(!Guid.TryParse(eventId, out eventguid))
                return;

            //update the game
            await _gameLogic.AddGame(game);
            //update the event type position
            await UpdatePosition(position);
            //update the processed events
            await AddEventTracked(eventId);
            //save the changes
            _context.SaveChanges();
        }
    }
}
