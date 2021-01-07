using System;

using GameApi.Adapters;
using GameApi.Entities;
using GameApi.Repositories;

using TbspRpgLib.EventProcessors;
using TbspRpgLib.Aggregates;
using TbspRpgLib.Settings;

using Microsoft.EntityFrameworkCore;

namespace GameApi.EventProcessors
{
    public class MyNewGameEventProcessor : NewGameEventProcessor
    {
        private IGameAggregateAdapter _gameAdapter;
        private IGameRepository _gameRepository;

        public MyNewGameEventProcessor(IEventStoreSettings eventStoreSettings, IDatabaseSettings databaseSettings) :
            base("game", eventStoreSettings, databaseSettings){
            _gameAdapter = new GameAggregateAdapter();

            //need to create a db context
            var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
            var optionsBuilder = new DbContextOptionsBuilder<GameContext>();
            optionsBuilder.UseNpgsql(connectionString);
            var context = new GameContext(optionsBuilder.Options);
            _gameRepository = new GameRepository(context);
        }

        protected override void HandleEvent(Aggregate aggregate, string eventId, ulong position) {
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
            if(game.UserId == null || game.Adventure == null || game.UserId == game.Adventure.Id)
                return;
            Console.WriteLine($"Writing Game {game.Id} {position}!!");
            Guid eventguid;
            if(!Guid.TryParse(eventId, out eventguid))
                return;
            game.Events.Add(new GameEvent() { Id = eventguid });
            _gameRepository.InsertGameIfDoesntExist(game);

            //update the event index, if this fails it's not a big deal
            //we'll end up reading duplicates
            UpdatePosition(position);
            return;
        }
    }
}
