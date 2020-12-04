using System;
using System.Text;
using System.Text.Json;

using EventStore.Client;

using GameApi.Aggregates;

namespace GameApi.Events
{
    public abstract class Event {
        public const string NEW_GAME_EVENT_TYPE = "new_game";

        public Event() {
            EventId = Guid.NewGuid();
            EventStoreUuid = Uuid.FromGuid(EventId);
        }

        public Guid EventId { get; set; }

        public Uuid EventStoreUuid { get; set; }

        public ulong Position { get; set; }

        public string Type { get; set; }

        protected abstract EventContent GetData();

        protected abstract void SetData(string jsonString);

        public abstract string GetDataJson();

        public abstract string GetStreamId();

        public abstract void UpdateAggregate(Aggregate agg);

        public static Event FromEventStoreEvent(ResolvedEvent resolvedEvent) {
            Event evnt;
            if(resolvedEvent.Event.EventType.StartsWith('$')) {
                return null;
            }
            
            //I'm not really happy with this part
            switch(resolvedEvent.Event.EventType) {
                case NEW_GAME_EVENT_TYPE:
                    evnt = new NewGameEvent();
                    break;
                default:
                    return null;
            }
            
            string jsonData = Encoding.UTF8.GetString(resolvedEvent.Event.Data.ToArray());
            evnt.SetData(jsonData);
            evnt.EventStoreUuid = resolvedEvent.Event.EventId;
            evnt.EventId = evnt.EventStoreUuid.ToGuid();
            evnt.Position = resolvedEvent.Event.Position.PreparePosition;
            return evnt;
        }

        public EventData ToEventStoreEvent() {
            return new EventData(
                EventStoreUuid,
                Type, 
                Encoding.UTF8.GetBytes(GetDataJson())
            );
        }
    }
}