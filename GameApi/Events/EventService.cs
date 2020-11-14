//this will abstract sending events to EventStore,
//so can change to different event store if necessary
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using EventStore.Client;

using TbspRgpLib.Settings;

namespace GameApi.Events
{
    public interface IEventService
    {
        void SendEvent(Event evnt, bool newStream);
        void SubscribeByType(string typeName);
    }

    public class EventService : IEventService {
        private IEventStoreSettings _eventStoreSettings;
        private EventStoreClient _eventStoreClient;

        public EventService(IEventStoreSettings eventStoreSettings) {
            _eventStoreSettings = eventStoreSettings;
            string esUrl = $"{_eventStoreSettings.Url}:{_eventStoreSettings.Port}";
            EventStoreClientSettings settings = new EventStoreClientSettings {
                ConnectivitySettings = {
                    Address = new Uri(esUrl)
                }
            };
            //may be able to put the settings in my config without the eventStoreSettings middleman
            _eventStoreClient = new EventStoreClient(settings);
        }

        public void SendEvent(Event evnt, bool newStream) {
            StreamState state;
            if(newStream) {
                state = StreamState.NoStream;
            } else {
                state = StreamState.Any;
            }
            
            _eventStoreClient.AppendToStreamAsync(
                evnt.GetStreamId(),
                state,
                new List<EventData> {
                    evnt.ToEventStoreEvent()
                }
            );
        }

        //Func<StreamSubscription, ResolvedEvent, CancellationToken, Task>
        public async void SubscribeByType(string typeName) {
            await _eventStoreClient.SubscribeToAllAsync(
                (sub, evnt, cancelToken) => {
                    Console.WriteLine(evnt.ToString());
                    return Task.CompletedTask;
                },
                filterOptions: new SubscriptionFilterOptions(
                    EventTypeFilter.Prefix($"{typeName}-")
                )
            );
        }
    }
}