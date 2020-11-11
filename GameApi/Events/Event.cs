using System;
using System.Text;
using System.Text.Json;

using EventStore.Client;

namespace GameApi.Events
{
    public abstract class Event {
        public const string NEW_GAME_EVENT_TYPE = "new_game";

        public Event() {
            EventId = Guid.NewGuid();
            EventStoreUuid = Uuid.FromGuid(EventId);
        }

        public override string ToString() {
            return $"{EventId}\n{Type}\n{JsonSerializer.Serialize(Data)}";
        }

        public Guid EventId { get; set; }

        public Uuid EventStoreUuid { get; set; }

        public string Type { get; set; }

        public object Data { get; set; }

        public abstract void InitEvent(object obj);

        public EventData ToEventStoreEvent() {
            return new EventData(
                EventStoreUuid,
                Type, 
                Encoding.UTF8.GetBytes(JsonSerializer.Serialize(Data))
            );
        }

        public abstract string GetAggregateId();
    }
}