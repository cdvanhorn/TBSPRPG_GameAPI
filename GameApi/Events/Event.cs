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

        public AggregateEnvelope Data { get; set; }

        public void InitNewGameEvent(Aggregate data) {
            Type = NEW_GAME_EVENT_TYPE;
            InitEvent(data);
        }

        private void InitEvent(Aggregate data) {
            //put the aggregate in an envelope
            AggregateEnvelope envelope = new AggregateEnvelope();
            envelope.Aggregate = data;
            envelope.EventIds.Add(EventId.ToString());
            Data = envelope;
        }

        public EventData ToEventStoreEvent() {
            return new EventData(
                EventStoreUuid,
                Type, 
                Encoding.UTF8.GetBytes(JsonSerializer.Serialize(Data))
            );
        }

        public string GetAggregateId() {
            return Data.Aggregate.Id;
        }

        public override string ToString() {
            return $"{EventId}\n{Type}\n{JsonSerializer.Serialize(Data)}";
        }
    }
}