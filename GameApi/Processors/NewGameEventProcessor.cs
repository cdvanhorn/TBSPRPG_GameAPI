using System;
using System.Threading.Tasks;

using GameApi.Events;
using GameApi.Aggregates;
using GameApi.Adapters;
using GameApi.Entities;
using GameApi.Repositories;
using GameApi.Services;

using TbspRgpLib.Settings;

namespace GameApi.Processors
{
    public class NewGameEventProcessor : EventProcessor
    {
        private IEventService _eventService;
        private IAggregateService _aggregateService;
        private IGameAggregateAdapter _gameAdapter;
        private IGameRepository _gameRepository;

        public NewGameEventProcessor(IEventStoreSettings eventStoreSettings, IDatabaseSettings databaseSettings) {
            _eventService = new EventService(eventStoreSettings);
            _aggregateService = new AggregateService(_eventService);
            _gameAdapter = new GameAggregateAdapter();
            _gameRepository = new GameRepository(databaseSettings);
        }

        protected override void PreTask()
        {
            _aggregateService.SubscribeByType(
                Event.NEW_GAME_EVENT_TYPE,
                (aggregate, eventId) => {
                    HandleEvent(aggregate, eventId);
                }
            );
        }

        private void HandleEvent(Aggregate aggregate, string eventId) {
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
            Console.WriteLine($"Writing Game {game.Id}!!");
            game.Events.Add(eventId);
            _gameRepository.InsertGameIfDoesntExist(game, eventId);
            return;
        }
    }
}
