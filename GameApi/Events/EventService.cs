//this will abstract sending events to EventStore,
//so can change to different event store if necessary
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using EventStore.Client;

using GameApi.Aggregates;

using TbspRgpLib.Settings;

namespace GameApi.Events
{
    public interface IEventService
    {
        void SendEvent(Event evnt, bool newStream);
        void SubscribeByType(string typeName, Action<Event> eventHandler);
        Task<List<Event>> GetEventsInStreamAsync(string streamId);
        Task<Aggregate> CreateAggregate(string aggregateId, string aggregateTypeName);
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

        public async void SubscribeByType(string typeName, Action<Event> eventHandler) {
            await _eventStoreClient.SubscribeToAllAsync(
                (subscription, evnt, token) => {
                    eventHandler(Event.FromEventStoreEvent(evnt));
                    return Task.CompletedTask;
                },
                filterOptions: new SubscriptionFilterOptions(
	                EventTypeFilter.Prefix(typeName)
                )
            );
        }

        public async Task<List<Event>> GetEventsInStreamAsync(string streamId) {
            var results = _eventStoreClient.ReadStreamAsync(
                Direction.Forwards,
                streamId,
                StreamPosition.Start);
            
            if(await results.ReadState == ReadState.StreamNotFound) {
                throw new ArgumentException($"invalid stream id {streamId}");
            }
            List<Event> events = new List<Event>();
            await foreach(var evnt in results) {
                events.Add(Event.FromEventStoreEvent(evnt));
            }
            return events;
        }

        public async Task<Aggregate> CreateAggregate(string aggregateId, string aggregateTypeName) {
            //create a new aggregate of the appropriate type
            string fqname = $"GameApi.Aggregates.{aggregateTypeName}";
            Type aggregateType = Type.GetType(fqname);
            if(aggregateType == null)
                throw new ArgumentException($"invalid aggregate type name {aggregateTypeName}");
            Aggregate aggregate = (Aggregate)Activator.CreateInstance(aggregateType);

            //get all of the events in the aggregrate id stream
            var events = await GetEventsInStreamAsync(aggregateId);
            foreach(var evnt in events) {
                evnt.UpdateAggregate(aggregate);
            }
            return aggregate;
        }
    }
}