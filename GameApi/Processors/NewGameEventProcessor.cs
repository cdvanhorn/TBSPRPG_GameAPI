using System;
using System.Threading.Tasks;

using GameApi.Events;

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
                    return HandleEvent(evnt);
                }
            );
        }

        private Task HandleEvent(Event evnt) {
            Console.WriteLine("Received Event: " + evnt);
            return Task.CompletedTask;
        }
    }
}
