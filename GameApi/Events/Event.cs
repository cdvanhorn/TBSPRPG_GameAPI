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

        public string Type { get; set; }

        public abstract EventContent GetData();

        public abstract string GetDataJson();

        public abstract string GetStreamId();

        public abstract void UpdateAggregate(Aggregate agg);

        public EventData ToEventStoreEvent() {
            return new EventData(
                EventStoreUuid,
                Type, 
                Encoding.UTF8.GetBytes(GetDataJson())
            );
        }
    }
}