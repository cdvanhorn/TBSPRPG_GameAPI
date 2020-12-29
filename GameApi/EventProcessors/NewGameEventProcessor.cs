using System;

using GameApi.Adapters;
using GameApi.Entities;
using GameApi.Repositories;

using TbspRpgLib.EventProcessors;
using TbspRpgLib.Aggregates;
using TbspRpgLib.Settings;

namespace GameApi.EventProcessors
{
    public class MyNewGameEventProcessor : NewGameEventProcessor
    {
        private IGameAggregateAdapter _gameAdapter;
        private IGameRepository _gameRepository;

        public MyNewGameEventProcessor(IEventStoreSettings eventStoreSettings, IDatabaseSettings databaseSettings) :
            base("game", eventStoreSettings, databaseSettings){
            _gameAdapter = new GameAggregateAdapter();
            _gameRepository = new GameRepository(databaseSettings);
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
            game.Events.Add(eventId);
            _gameRepository.InsertGameIfDoesntExist(game, eventId);

            //update the event index, if this fails it's not a big deal
            //we'll end up reading duplicates
            UpdatePosition(position);
            return;
        }
    }
}
