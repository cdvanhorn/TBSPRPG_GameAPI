using System;
using System.Linq;
using System.Threading.Tasks;


using GameApi.Adapters;
using GameApi.Entities;
using GameApi.Repositories;
using GameApi.Services;

using TbspRpgLib.Repositories;
using TbspRpgLib.Entities;
using TbspRpgLib.Events;
using TbspRpgLib.Aggregates;
using TbspRpgLib.Settings;

namespace GameApi.Processors
{
    public class NewGameEventProcessor : EventProcessor
    {
        private IEventService _eventService;
        private IAggregateService _aggregateService;
        private IGameAggregateAdapter _gameAdapter;
        private IGameRepository _gameRepository;
        private IServiceRespository _serviceRepository;
        private Task<Service> _serviceTask;
        private Service _service;

        public NewGameEventProcessor(IEventStoreSettings eventStoreSettings, IDatabaseSettings databaseSettings) {
            _eventService = new EventService(eventStoreSettings);
            _aggregateService = new AggregateService(_eventService);
            _gameAdapter = new GameAggregateAdapter();
            _gameRepository = new GameRepository(databaseSettings);
            _serviceRepository = new ServiceRepository(databaseSettings);
            _serviceTask = _serviceRepository.GetServiceByName("game");
        }

        protected override async void PreTask()
        {
            //get where we need to start reading from
            _service = await _serviceTask;
            EventIndex ei = _service.EventIndexes.Where(ei => ei.EventName == Event.NEW_GAME_EVENT_TYPE).FirstOrDefault();
            ulong startPosition = 0;
            if(ei != null && ei.Index > 0)
                startPosition = ei.Index;
            Console.WriteLine($"Start position: {startPosition}");

            _aggregateService.SubscribeByType(
                Event.NEW_GAME_EVENT_TYPE,
                (aggregate, eventId, position) => {
                    HandleEvent(aggregate, eventId, position);
                },
                startPosition
            );
        }

        private void HandleEvent(Aggregate aggregate, string eventId, ulong position) {
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
            var eventIndex = _service.EventIndexes.Where(ei => ei.EventName == Event.NEW_GAME_EVENT_TYPE).First();
            if(eventIndex.Index < position) {
                eventIndex.Index = position;
                _serviceRepository.UpdateService(_service, Event.NEW_GAME_EVENT_TYPE);
            }
            return;
        }
    }
}
