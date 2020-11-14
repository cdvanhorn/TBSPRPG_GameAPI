using System.Text.Json;

using GameApi.Aggregates;
using GameApi.Events.Content;

namespace GameApi.Events {
    public class NewGameEvent : Event {
        public NewGame Data { get; set; }

        public NewGameEvent(NewGame data) : base() {
            Type = NEW_GAME_EVENT_TYPE;
            Data = data;
        }

        public override void UpdateAggregate(Aggregate agg) {
            
        }

        public override EventContent GetData() {
            return Data;
        }

        public override string GetStreamId() {
            return Data.Id;
        }

        public override string ToString() {
            return $"{EventId}\n{Type}\n{JsonSerializer.Serialize(Data)}";
        }
    }
}