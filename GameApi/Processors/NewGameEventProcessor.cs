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
            _eventService.SubscribeByType("new_game");
        }
    }
}
