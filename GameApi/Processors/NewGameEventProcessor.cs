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

        public NewGameEventProcessor(IEventStoreSettings eventStoreSettings) {
            _eventService = new EventService(eventStoreSettings);
        }

        protected override void PreTask()
        {
            _eventService.SubscribeByType(
                Event.NEW_GAME_EVENT_TYPE,
                (evnt) => {
                    HandleEvent(evnt);
                }
            );
        }

        private async void HandleEvent(Event evnt) {
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

            //this is a new game event I know the aggregate in here is
            //a game aggregate
            //we need to get the id of the aggregate so we can load it's stream
            var aggregateId = evnt.GetStreamId();
            if(aggregateId == null) //we can't parse this event
                return;
            var aggTask = _eventService.CreateAggregate(aggregateId, "GameAggregate");
            GameAggregate gameAggregate = (GameAggregate)await aggTask;
            
            Console.WriteLine("Received Event: " + evnt);
            return;
        }
    }
}
