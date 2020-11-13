using System;
using System.Text;
using System.Text.Json;

using EventStore.Client;

using GameApi.Aggregates;

namespace GameApi.Events
{
    public class Event {
        public const string NEW_GAME_EVENT_TYPE = "new_game";

        public Event() {
            EventId = Guid.NewGuid();
            EventStoreUuid = Uuid.FromGuid(EventId);
        }

        public Guid EventId { get; set; }

        public Uuid EventStoreUuid { get; set; }

        public string Type { get; set; }

        public EventData Data { get; set; }

        public EventData ToEventStoreEvent() {
            // return new EventData(
            //     EventStoreUuid,
            //     Type, 
            //     Encoding.UTF8.GetBytes(JsonSerializer.Serialize(Data))
            // );
            return null;
        }

        public string GetStreamId() {
            return Data.Id;
        }

        public override string ToString() {
            return $"{EventId}\n{Type}\n{JsonSerializer.Serialize(Data)}";
        }
    }
}