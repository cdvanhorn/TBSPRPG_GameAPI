using GameApi.Events;

namespace GameApi.Events.Data {
    public class NewGame : EventData {
        public string UserId { get; set; }

        public string AdventureName { get; set; }

        public string AdventureId { get; set; }
    }
}