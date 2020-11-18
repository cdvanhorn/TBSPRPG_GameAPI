using System;
using System.Threading.Tasks;

using GameApi.Events;
using GameApi.Aggregates;

using TbspRgpLib.Settings;

namespace GameApi.Processors
{
    public class NewGameEventProcessor : EventProcessor
    {
        private IEventService _eventService;
        private IAggregateService _aggregateService;

        public NewGameEventProcessor(IEventStoreSettings eventStoreSettings) {
            _eventService = new EventService(eventStoreSettings);
            _aggregateService = new AggregateService(_eventService);
        }

        protected override void PreTask()
        {
            _aggregateService.SubscribeByType(
                Event.NEW_GAME_EVENT_TYPE,
                (aggregate) => {
                    HandleEvent(aggregate);
                }
            );
        }

        private void HandleEvent(Aggregate aggregate) {
            //generate related aggregate from it's stream
            //this db loading processor so check if this event
            //id is in the data base
            //otherwise would check the aggregate list of event ids
            //if the event id is already there we processed it and can
            //skip this event
            //At this point we can process the event
            //for this event since it's a new object we can see if the id
            //is already there if so we can skip this event
            //otherwise we write the game to the database
            Console.WriteLine("Received Aggregate: " + aggregate);
            return;
        }
    }
}
