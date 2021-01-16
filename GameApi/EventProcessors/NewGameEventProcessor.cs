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

namespace GameApi.EventProcessors
{
    public class MyNewGameEventProcessor : NewGameEventProcessor
    {
        private IGameAggregateAdapter _gameAdapter;
        private IGameLogic _gameLogic;
        private GameContext _context;

        public MyNewGameEventProcessor(IEventStoreSettings eventStoreSettings) :
            base("game", eventStoreSettings){
            _gameAdapter = new GameAggregateAdapter();

            //need to create a db context
            var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
            var optionsBuilder = new DbContextOptionsBuilder<GameContext>();
            optionsBuilder.UseNpgsql(connectionString);
            _context = new GameContext(optionsBuilder.Options);

            //create the game service
            GameRepository gameRepository = new GameRepository(_context);
            EventService eventService = new EventService(eventStoreSettings);
            GameService gameService = new GameService(gameRepository,
                new EventAdapter(),
                eventService
            );

            //create the adventure service
            AdventureRepository adventureRepository = new AdventureRepository(_context);
            AdventureService adventureService = new AdventureService(adventureRepository);

            _gameLogic = new GameLogic(
                new EventAdapter(),
                eventService,
                gameService,
                adventureService
            );
            
            //initialize the services
            this.InitializeServices(_context);
        }

        protected override async void HandleEvent(Aggregate aggregate, string eventId, ulong position) {
            //generate related aggregate from it's stream
            //this db loading processor so check if this event
            //id is in the data base
            //otherwise would check the aggregate list of event ids
            //if the event id is already there we processed it and can
            //skip this event
            GameAggregate gameAggregate = (GameAggregate)aggregate;
            //we need to convert the aggregate to a game object and insert in to the database
            //but we don't want to insert duplicates
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
